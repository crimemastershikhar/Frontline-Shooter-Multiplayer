using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject loadingScreen;
    public GameObject menuButtons;
    public TMP_Text loadingText;

    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ClosedMenus();
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to network...";

        PhotonNetwork.ConnectUsingSettings();
    }

    private void ClosedMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby()
    {
        ClosedMenus();
        menuButtons.SetActive(true);
    }
}
