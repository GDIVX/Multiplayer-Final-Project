using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] TMP_Text _playerText, _sizeText, _ScoreText;
    internal void SetEntry(string playerName, int score, float size)
    {
        _playerText.text = playerName;
        _sizeText.text = size.ToString();
        _ScoreText.text = score.ToString();
    }


}
