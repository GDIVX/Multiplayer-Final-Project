using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    List<PhotonView> trackedPlayers = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void AddActivePlayer(PhotonView player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        if (!trackedPlayers.Contains(player))
        {
            trackedPlayers.Add(player);
        }
    }

    [PunRPC]
    public void RemoveActivePlayer(PhotonView player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (trackedPlayers.Contains(player))
        {
            trackedPlayers.Remove(player);

            //if only one player is left, they win
            if (trackedPlayers.Count == 1)
            {
                //get the player that won
                PhotonView winner = trackedPlayers[0];

                //tell all clients that the game is over
                photonView.RPC("GameOver", RpcTarget.All, winner);
            }
        }
    }

    public void OnEaten(PhotonView photonView)
    {
        if (photonView is null)
        {
            return;
        }

        // Assuming your Player class has a PhotonView property
        foreach (var player in trackedPlayers)
        {
            if (player.ViewID == photonView.ViewID)
            {
                // If the eaten object is a player, remove them from the active players
                // It's important to not modify a list while iterating over it, so instead mark the player for removal
                RemoveActivePlayer(player);
                break;
            }
        }


    }

    [PunRPC]
    void GameOver(PhotonView winner)
    {
        //do something with the winner
        Debug.Log($"Player {winner.Owner.NickName} won!");
    }


}
