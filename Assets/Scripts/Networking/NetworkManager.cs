using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //Instance
    public static NetworkManager instance;

    const int OVERALL_MAX_PLAYERS = 6;
    const int MIN_MAX_PLAYERS = 2;

    public void Awake()
    {
        //Set instance
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        //Connect to master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string roomName, int maxPlayers)
    {
        maxPlayers = Mathf.Clamp(maxPlayers, MIN_MAX_PLAYERS, OVERALL_MAX_PLAYERS);

        PhotonNetwork.CreateRoom(roomName, roomOptions: new RoomOptions { MaxPlayers = maxPlayers });
    }

    public bool TryToJoinRoom(string roomName)
    {
        return PhotonNetwork.JoinRandomRoom();
    }


    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master Server!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined {PhotonNetwork.CurrentRoom.Name}!");
    }
}
