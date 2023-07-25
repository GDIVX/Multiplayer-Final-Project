using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviourPun
{

    public static ObjectSpawner Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Button]
    public void SpawnObject(string name, Vector3 pos, Quaternion rot)
    {
        PhotonNetwork.Instantiate(name, pos, rot);
    }



}
