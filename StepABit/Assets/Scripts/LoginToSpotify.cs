using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginToSpotify : MonoBehaviour
{
    public static string URL = "https://accounts.spotify.com/authorize?response_type=code&client_id=0bfca826fca245c7914b0592f5d04475&scope=user-read-private playlist-modify-private user-follow-read ugc-image-upload ugc-image-upload user-modify-playback-state user-read-currently-playing user-read-email user-follow-modify user-library-modify user-library-read streaming app-remote-control user-read-playback-position user-top-read user-read-recently-played playlist-read-collaborative playlist-read-private playlist-modify-public&redirect_uri=com.univ.redirectcatcher://oauth/callback/spotify/";

    public void SendAuthRequest()
    {
        Application.OpenURL(URL);
    }


    public IEnumerator OnAccessToken(string accessToken)
    {
        Debug.Log("HELLO");
        Debug.Log(accessToken);


        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", accessToken);
        form.AddField("redirect_uri", "com.univ.redirectcatcher://oauth/callback/spotify/");


        UnityWebRequest www = UnityWebRequest.Post("https://accounts.spotify.com/api/token", form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Authorization", "Basic MGJmY2E4MjZmY2EyNDVjNzkxNGIwNTkyZjVkMDQ0NzU6ZDQ5YzFjZmQ1MTQ2NGM4N2EwOTIxYzA5ZTc0ZWM3NDM=");
        yield return www.SendWebRequest();

        var data = www.downloadHandler.text;

        string code = data.Substring(data.IndexOf("\"access_token\"") + "\"access_token\":\"".Length);
        code = code.Substring(0, code.IndexOf("\"")); 
        PlayerPrefs.SetString("AuthCode", code);
        Debug.Log(data);
        Debug.Log(code);
        StartCoroutine(SayHello());
    }

    public IEnumerator SayHello()
    {

        WWWForm form = new WWWForm();
        form.AddField("name", "Ukrainized");
        //TODO: HEADER WITH AUTHCODE
        UnityWebRequest www = UnityWebRequest.Post("https://api.spotify.com/v1/users/i2mxmi6yazaqwx33ww04t19p8/playlists", form);
        yield return www.SendWebRequest();

    }
}
