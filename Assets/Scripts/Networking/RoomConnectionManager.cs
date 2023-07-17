using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

public class RoomConnectionManager : MonoBehaviour
{
    [SerializeField] TMP_InputField roomName_inputField, maxPlayers_InputField;

    static NetworkManager NetworkManager => NetworkManager.instance;

    public UnityEvent<string> OnJoinToRoomFailed;
    public UnityEvent<string> OnCreateRoomFailed;

    public void CreateRoom(string roomName, int maxPlayers)
    {
        NetworkManager.CreateRoom(roomName, maxPlayers);
    }

    public void CreateRoom(TMP_InputField roomName_inputField, TMP_InputField maxPlayers_inputField)
    {
        if (roomName_inputField is null)
        {
            throw new ArgumentNullException(nameof(roomName_inputField));
        }

        if (maxPlayers_inputField is null)
        {
            throw new ArgumentNullException(nameof(maxPlayers_inputField));
        }



        string roomName = roomName_inputField.text;
        int maxPlayers = int.Parse(maxPlayers_inputField.text);

        if (roomName.IsNullOrEmpty())
        {
            OnCreateRoomFailed?.Invoke("Must enter a room name");
            return;
        }

        NetworkManager.CreateRoom(roomName, maxPlayers);
    }

    public bool TryToJoinRoom(string roomName)
    {
        if (!NetworkManager.TryToJoinRoom(roomName))
        {
            OnJoinToRoomFailed.Invoke(roomName);
            return false;
        }

        return true;
    }

    public bool TryToJoinRoom(TMP_InputField roomName_inputField)
    {
        string roomName = roomName_inputField.text;

        return TryToJoinRoom(roomName);
    }

    public void JoinRoom()
    {
        TryToJoinRoom(roomName_inputField);
    }

    public void CreateRoom()
    {
        CreateRoom(roomName_inputField, maxPlayers_InputField);
    }
}
