using Photon.Pun;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets.Scripts.GameManager;

public class LeaderboardNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] LeaderboardUI _leaderboardUI;


    [PunRPC]
    public void RpcReceiveLeaderboard(string jsonLeaderboard)
    {
        Wrapper<PlayerData> leaderboardData = JsonUtility.FromJson<Wrapper<PlayerData>>(jsonLeaderboard);

        foreach (var playerData in leaderboardData.Items)
        {
            string playerName = playerData.playerName;
            int score = playerData.score;
            float size = playerData.size;

            _leaderboardUI.AddEntry(playerName, score, size);
        }
    }

    public void FetchLeaderboard()
    {
        var scoreTable = Leaderboard.Instance.ScoreTable;
        var sizeTable = Leaderboard.Instance.SizeTable;

        List<PlayerData> leaderboardData = new List<PlayerData>();

        foreach (var player in scoreTable.Keys)
        {
            leaderboardData.Add(new PlayerData(player, scoreTable[player], sizeTable[player]));
        }

        string jsonLeaderboard = JsonUtility.ToJson(new Wrapper<PlayerData> { Items = leaderboardData });

        Debug.Log(jsonLeaderboard);

        // Call the RPC method on all clients, passing the JSON string as an argument.
        photonView.RPC("RpcReceiveLeaderboard", RpcTarget.All, jsonLeaderboard);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public float size;

    public PlayerData(string playerName, int score, float size)
    {
        this.playerName = playerName;
        this.score = score;
        this.size = size;
    }
}