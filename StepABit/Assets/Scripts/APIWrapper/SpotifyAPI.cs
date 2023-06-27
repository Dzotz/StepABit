using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class SpotifyAPI : MonoBehaviour
{
    public WebRequestHandler handler;
    public static SpotifyAPI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public IEnumerator GetUsersSaved(Action<List<string>> result)
    {
        List<string> usersSaved = new List<string>();
        int limit = 0;
        Debug.Log("STARTED GET COUNT");
        yield return StartCoroutine(handler.GetUserSavedCount("UA", 1, 0, value => limit = value));
        Debug.Log("COUNT: "+limit);
        if(limit > 0)
        {
            int offset = 0;
            List<string> savedTracks = new List<string>();
            while(limit > 0)
            {
                if (limit > 50)
                {
                    //Debug.Log("CURRENT: " + limit);
                    yield return StartCoroutine(handler.GetUserSaved("UA", 50, offset, value => savedTracks = new List<string>(value)));
                    offset += 50;
                    limit -= 50;
                    usersSaved.AddRange(savedTracks);
                }
                else
                {
                    //Debug.Log("CURRENT: " + limit);
                    yield return StartCoroutine(handler.GetUserSaved("UA", limit, offset, value => savedTracks = new List<string>(value)));
                    offset += limit;
                    limit -= limit;
                    usersSaved.AddRange(savedTracks);
                }
            }
        }
        result(usersSaved);
    
    }

    public IEnumerator GetTracksBPM(List<string> tracks, Action<List<double>> result)
    {
        List<double> tracksBPM = new List<double>();

        int limit = tracks.Count;

        while (limit > 0)
        {
            if (limit > 100)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i=0; i<99; i++)
                {
                    stringBuilder.Append(tracks[i]);
                    stringBuilder.Append(',');
                }
                stringBuilder.Append(tracks[99]);
                List<double> tmpRes = new List<double>();
                yield return StartCoroutine(handler.GetTracksBPM(stringBuilder.ToString(), value => tmpRes = new List<double>(value)));
                tracksBPM.AddRange(tmpRes);
                limit -= 100;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < limit-1; i++)
                {
                    stringBuilder.Append(tracks[i]);
                    stringBuilder.Append(',');
                }
                stringBuilder.Append(tracks[limit-1]);
                List<double> tmpRes = new List<double>();
                yield return StartCoroutine(handler.GetTracksBPM(stringBuilder.ToString(), value => tmpRes = new List<double>(value)));
                tracksBPM.AddRange(tmpRes);
                limit -= limit;
            }
        }

        result(tracksBPM);
    }

    public IEnumerator CreatePlaylist(string name, List<string> tracks, Action<bool> success)
    {
        bool successCode = false;
        string userId = "";
        yield return StartCoroutine(handler.GetUserId(value => userId = value));

        Debug.Log("USER ID " + userId);
        string playlistId = "";
        yield return StartCoroutine(handler.PostCreatePlaylist(userId, name, value => playlistId = value));
       

        List<string> tracksURIs = new List<string>();

        for(int i=0; i<tracks.Count; i++)
        {
            tracksURIs.Add("spotify:track:"+tracks[i]);
        }
        int lim = tracks.Count>50?50:tracks.Count;
        List<string> tmp = tracksURIs.GetRange(0, lim);
        StartCoroutine(handler.PostAddTracks(playlistId, tmp, value => successCode = value));

        success(successCode);
    }

    public IEnumerator GetReferenceVals(string trackId, Action<List<String>> result)
    {
        List<string> res = new List<string>();
        string author ="";
        yield return StartCoroutine(handler.GetTrackAuthor(trackId, value => author = value));

        Debug.Log("AUTHOR REF" + author);
        res.Add(author);

        List<string> genres = new List<string>();

        yield return StartCoroutine(handler.GetGenres(value => genres = new List<string>(value)));

        res.AddRange(genres);
        res.Add(trackId);
        foreach(string s in res)
        {
            Debug.Log("RESULT REF" + s);
        }

        result(res);
    }

    public IEnumerator GetRecommendations(int count, double bpm, string trackId, string author, string genres, Action<List<string>> result)
    {
        List<string> res = new List<string>();

        yield return StartCoroutine(handler.GetRecomendations(count, bpm, trackId, author, genres, value => res = new List<string>(value)));
    
        result(res);
    }

}
