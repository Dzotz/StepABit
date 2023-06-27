using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    public GameObject SpotifyPanel, MainPanel, SetABitPanel, MeasureBitPanel;

    public void EnterSpotifyPanel()
    {
        MeasureBitPanel.SetActive(false);
        SpotifyPanel.SetActive(true);
        SetABitPanel.SetActive(false);
        MainPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        MeasureBitPanel.SetActive(false);
        SpotifyPanel.SetActive(false);
        SetABitPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
}
