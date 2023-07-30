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

    public void AddEntry(string playerName, int score, float fishSize)
    {
        if (_scoreTable.ContainsKey(playerName))
        {
            _scoreTable[playerName] += score;
            _sizeTable[playerName] += fishSize;
        }
        else
        {
            _scoreTable.Add(playerName, score);
            _sizeTable.Add(playerName, fishSize);
        }

    }

    public (int, float) GetEntry(string playerName)
    {
        return _scoreTable.ContainsKey(playerName) && _sizeTable.ContainsKey(playerName)
            ? (_scoreTable[playerName], _sizeTable[playerName])
            : (0, 0);
    }
}
