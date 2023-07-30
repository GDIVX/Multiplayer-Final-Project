using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [SerializeField] List<PhotonView> trackedPlayers;
    [SerializeField] TMP_Text playerWonText;
    [SerializeField] Transform gameOverUI;
    [SerializeField] Button exitButton;

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

    private void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            NetworkManager.instance.LeaveRoom();
        });
    }

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

    public void RemoveActivePlayer(PhotonView player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (!trackedPlayers.Contains(player))
        {
            return;
        }

        trackedPlayers.Remove(player);


        //if only one player is left, they win
        if (trackedPlayers.Count == 1)
        {
            //get the player that won
            PhotonView winner = trackedPlayers.First();

            //tell all clients that the game is over
            photonView.RPC("GameOver", RpcTarget.All, winner.ViewID);
        }



    }

    public void OnPlayerEaten(PhotonView photonView)
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
    void GameOver(int winnerViewID)
    {
        gameOverUI.gameObject.SetActive(true);

        PhotonView winner = PhotonView.Find(winnerViewID);
        if (winner == null)
        {
            Debug.LogError($"Can't find photon view object for ID {winnerViewID} ");
            return;
        }

        if (winner.Owner == null)
        {
            Debug.LogError($"photon view with ID {winnerViewID} don't have an owner");
            return;
        }

        playerWonText.text = $"{winner.Owner.NickName} won!";
    }

    [PunRPC]
    void DestroyObject(int viewID)
    {
        PhotonView obj = PhotonView.Find(viewID);
        if (obj != null)
        {
            PhotonNetwork.Destroy(obj);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log(trackedPlayers.Count);
            int[] viewIDs = trackedPlayers.Select(pv => pv.ViewID).ToArray();
            photonView.RPC("TransferTrackedPlayersList", newMasterClient, viewIDs);
        }
    }


    [PunRPC]
    void TransferTrackedPlayersList(int[] newTrackedPlayerIDs)
    {


        foreach (int id in newTrackedPlayerIDs)
        {
            PhotonView photonView = PhotonView.Find(id);
            if (photonView == null)
            {
                Debug.LogWarning($"Can't find photon view for ID {photonView}");
                continue;
            }

            if (trackedPlayers.Contains(photonView))
            {
                continue;
            }

            trackedPlayers.Add(photonView);
        }
    }


}
