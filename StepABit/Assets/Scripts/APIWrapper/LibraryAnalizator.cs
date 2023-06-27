using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LibraryAnalizator : MonoBehaviour
{
    List<string> trackIds = new List<string>();
    List<double> trackBpm = new List<double>();

    List<string> refs = new List<string>();
    double bpm = 0;
    int findMore = 0;
    List<string> chosenTracks = new List<string>();
    Dictionary<string, double> tracks = new Dictionary<string, double>();
    bool success = false;

    public void GetTracks()
    {
        StartCoroutine(SpotifyAPI.Instance.GetUsersSaved(value => trackIds = new List<string>(value)));
        Invoke("GetBPMs", 30);
    }

    public void GetBPMs()
    {
        StartCoroutine(SpotifyAPI.Instance.GetTracksBPM(trackIds, value => trackBpm = new List<double>(value)));
        Invoke("Join", 30);
    }

    public void Join()
    {
        Debug.Log("===============================================");
        Debug.Log("IS EVERYTHIN OK? "+(trackBpm.Count==trackIds.Count)+ " "+ trackIds.Count);
        for(int i =0; i<trackIds.Count; i++)
        {
            if (tracks.ContainsKey(trackIds[i]))
            {
                continue;
            }
            tracks.Add(trackIds[i], trackBpm[i]);
        }

        foreach(var track in tracks)
        {
            Debug.Log("KEY: "+track.Key+" VALUE: "+ track.Value);
        }


        //DELETE LATER
        ChooseTracks(120);
    }

    public void ChooseTracks(double bpm)
    {
        Debug.Log("TARGET " + bpm);
        this.bpm = bpm;
        foreach(var track in tracks)
        {
            
            if(Math.Abs(track.Value-bpm)<10)
            {
                chosenTracks.Add(track.Key);
            }
        }
        
        if (chosenTracks.Count < 50)
        {
            if(chosenTracks.Count == 0)
            {
                chosenTracks.Add(trackIds[0]);
            }
            string id = chosenTracks[0];
            findMore = 50 - chosenTracks.Count;
            StartCoroutine(SpotifyAPI.Instance.GetReferenceVals(id, value => refs = new List<string>(value)));
            Invoke("GetRecomendations", 30);
        }
        else
        {
            Invoke("CreatePlaylist", 30);
        }
        //StartCoroutine(SpotifyAPI.Instance.CreatePlaylist("Step A bit " + bpm + " bpm Playlist", chosenTracks, value => success = value));
    }

    public void GetRecomendations()
    {
        List<string> recomendations = new List<string>();
        int genreCount = refs.Count - 2 > 3 ? 3 : refs.Count - 2;
        string genres = "";
        for(int i=0; i<genreCount-1; i++)
        {
            genres+=refs[1+i]+",";
        }
        genres += refs[genreCount];
        StartCoroutine(SpotifyAPI.Instance.GetRecommendations(findMore, bpm, refs[refs.Count - 1], refs[0], genres, value => chosenTracks.AddRange(value)));
        Invoke("CreatePlaylist", 30);
    }

    public void CreatePlaylist()
    {
        StartCoroutine(SpotifyAPI.Instance.CreatePlaylist("Step A bit " + bpm + " bpm Playlist", chosenTracks, value => success = value));
        Debug.Log("SUCCESS???? " + success);
    }
}
