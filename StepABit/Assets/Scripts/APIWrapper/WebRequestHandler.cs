using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text;

public class WebRequestHandler : MonoBehaviour
{
    public IEnumerator GetUserSaved(string market, int limit, int offset, Action<List<string>> result)
    {
        string url = "https://api.spotify.com/v1/me/tracks";
        url += "?market=" + market;
        url += "&limit=" + limit.ToString();
        url += "&offset=" + offset.ToString();

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer "+PlayerPrefs.GetString("AuthCode"));
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        List<string> res = new List<string>();
        JObject info = JObject.Parse(data);

        foreach (var item in info["items"])
        {
            res.Add(item["track"]["id"].ToString());
        }

        result(res);
    }

    public IEnumerator GetUserSavedCount(string market, int limit, int offset, Action<int> result)
    {
        string url = "https://api.spotify.com/v1/me/tracks";
        url += "?market=" + market;
        url += "&limit=" + limit.ToString();
        url += "&offset=" + offset.ToString();
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        Debug.Log(PlayerPrefs.GetString("AuthCode"));
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);
        int res;
        JObject info = JObject.Parse(data);
        
        res = Int32.Parse(info["total"].ToString());

        result(res);
    }

    public IEnumerator GetTracksBPM(string ids, Action<List<double>> result)
    {
        string url = "https://api.spotify.com/v1/audio-features?ids=" + ids;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        Debug.Log(PlayerPrefs.GetString("AuthCode"));
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        JObject info = JObject.Parse(data);
        List<double> res = new List<double>();
 
        foreach (var item in info["audio_features"])
        {
            res.Add(Convert.ToDouble(item["tempo"].ToString()));
        }

        result(res);
    }

    public IEnumerator GetUserId(Action<string> result)
    {
        string url = "https://api.spotify.com/v1/me";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        JObject info = JObject.Parse(data);
        string res = info["id"].ToString();

        Debug.Log(res);

        result(res);
    }

    public IEnumerator PostCreatePlaylist(string userId, string name, Action<string> result)
    {
        string url = "https://api.spotify.com/v1/users/" + userId + "/playlists";

        string form = "{\"name\":\""+name+"\", \"public\": false}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(form);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));

        yield return request.SendWebRequest();

        var data = request.downloadHandler.text;
        Debug.Log(data);

        JObject info = JObject.Parse(data);
        string res = info["id"].ToString();

        result(res);
    }

    public IEnumerator PostAddTracks(string playlistId, List<string> trackUris, Action<bool> result)
    {
        bool res;

        string url = "https://api.spotify.com/v1/playlists/" + playlistId + "/tracks";
        string form = "{\"uris\": ["; 
        for(int i = 0; i < trackUris.Count-1; i++)
        {
            form +="\""+ trackUris[i] + "\",";
        }
        form += "\""+trackUris[trackUris.Count-1]+"\"],\"position\":0}";

        Debug.Log(form);
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(form);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        res = www.responseCode == 201;

        result(res);
    }

    public IEnumerator GetTrackAuthor(string id, Action<string> result)
    {
        string url = "https://api.spotify.com/v1/tracks/" + id;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));

        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        JObject info = JObject.Parse(data);
        string author = info["artists"][0]["id"].ToString();
        
        result(author);
    }

    public IEnumerator GetGenres(Action<List<string>> result)
    {
        List<string> res = new List<string>();
        string url = "https://api.spotify.com/v1/recommendations/available-genre-seeds";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));

        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        JObject info = JObject.Parse(data);

        foreach (var item in info["genres"])
        {
            res.Add(item.ToString());
        }

        result(res);
    }

    public IEnumerator GetRecomendations(int count, double bpm, string trackId, string author, string genres, Action<List<string>> result)
    {
        List<string> res = new List<string>();

        string url = "https://api.spotify.com/v1/recommendations";
        int intBpm = (int)bpm;
        url += "?limit=" + count;
        url += "&market=UA";
        url += "&seed_artists=" + author;
        url += "&seed_genres=" + genres;
        url+= "&seed_tracks=" + trackId;
        url += "&min_tempo=" + (intBpm - 10);
        url += "&max_tempo=" + (intBpm + 10);
        url += "&target_tempo=" + intBpm;

        Debug.Log("URL " + url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthCode"));
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;
        Debug.Log(data);

        JObject info = JObject.Parse(data);

        foreach (var item in info["tracks"])
        {
            res.Add(item["id"].ToString());
        }

        result(res);
    }
}
