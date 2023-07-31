using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField] GameObject _entryPrefab;
        internal void AddEntry(string playerName, int score, float size)
        {
            //instantiate new entry prefab
            GameObject entry = Instantiate(_entryPrefab, transform);

            //set entry data
            entry.GetComponent<LeaderboardEntryUI>().SetEntry(playerName, score, size);

            //Add child to parent
            entry.transform.SetParent(transform);

        }
    }
}