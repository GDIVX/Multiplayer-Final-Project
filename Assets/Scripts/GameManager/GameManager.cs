using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [SerializeField] List<PhotonView> trackedPlayers;
    [SerializeField] TMP_Text playerWonText;
    [SerializeField] Transform gameOverUI;
    [SerializeField] LeaderboardNetworkManager leaderboardNetworkManager;
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

    public void AddActivePlayer(PhotonView view)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        if (!trackedPlayers.Contains(view))
        {
            trackedPlayers.Add(view);
        }
    }

    public void RemoveActivePlayer(PhotonView view)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (!trackedPlayers.Contains(view))
        {
            return;
        }

        trackedPlayers.Remove(view);


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

    [Button]
    void Debug_GameOver()
    {

        photonView.RPC("GameOver", RpcTarget.All, trackedPlayers[0].ViewID);

    }

    [PunRPC]
    void GameOver(int winnerViewID)
    {
        gameOverUI.gameObject.SetActive(true);

        //Update the leaderboard
        trackedPlayers.ForEach((view) => AddPlayerToLeaderboard(view));
        leaderboardNetworkManager.FetchLeaderboard();

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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int[] viewIDs = trackedPlayers.Where(pv => pv != null).Select(pv => pv.ViewID).ToArray();
                photonView.RPC("TransferTrackedPlayersList", RpcTarget.AllBuffered, viewIDs);
            }
        }
    }

    [PunRPC]
    void TransferTrackedPlayersList(int[] newTrackedPlayerIDs)
    {
        trackedPlayers.Clear();
        foreach (int id in newTrackedPlayerIDs)
        {
            PhotonView view = PhotonView.Find(id);
            if (view != null)
            {
                AddActivePlayer(view);
            }
        }
    }

    public void AddPlayerToLeaderboard(PhotonView view)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (view == null)
        {
            return;
        }

        if (view.Owner == null)
        {
            return;
        }

        if (view.Owner.NickName == null)
        {
            return;
        }


        float size = view.GetComponent<Transform>().localScale.x;
        int score = Mathf.RoundToInt(size * 100);

        Debug.Log($"Adding {view.Owner.NickName} to leader board. Score: {score} | Size: {size}");
        Leaderboard.Instance.AddEntry(view.Owner.NickName, score, size);
    }

    public void AddPlayerToLeaderboard(Player player)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        if (player.NickName == null)
        {
            return;
        }

        //Get the player's photon view
        PhotonView view = player.TagObject as PhotonView;

        //If the player has a photon view, add them to the leaderboard
        if (view != null)
        {
            float size = view.GetComponent<Transform>().localScale.x;
            int score = Mathf.RoundToInt(size * 100);

            Leaderboard.Instance.AddEntry(player.NickName, score, size);
        }

    }


}
