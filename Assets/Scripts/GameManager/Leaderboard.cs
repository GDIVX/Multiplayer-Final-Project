using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard
{
    //Singleton
    private static Leaderboard _instance;

    Dictionary<string, int> _scoreTable = new();
    Dictionary<string, float> _sizeTable = new();


    public static Leaderboard Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Leaderboard();
            }

            return _instance;
        }
    }

    public Dictionary<string, int> ScoreTable { get => _scoreTable; }
    public Dictionary<string, float> SizeTable { get => _sizeTable; }

    public void AddEntry(string playerName, int score, float fishSize)
    {
        if (ScoreTable.ContainsKey(playerName))
        {
            ScoreTable[playerName] += score;
            SizeTable[playerName] += fishSize;
        }
        else
        {
            ScoreTable.Add(playerName, score);
            SizeTable.Add(playerName, fishSize);
        }

    }

    public (int, float) GetEntry(string playerName)
    {
        return ScoreTable.ContainsKey(playerName) && SizeTable.ContainsKey(playerName)
            ? (ScoreTable[playerName], SizeTable[playerName])
            : (0, 0);
    }
}


