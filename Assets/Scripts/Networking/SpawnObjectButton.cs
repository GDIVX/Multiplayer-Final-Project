using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnObjectButton : MonoBehaviourPun
{
    [SerializeField] Button button;
    [SerializeField] Transform panel;
    [SerializeField, FilePath(ParentFolder = "Assets/Resources")] string prefabName;

    private void Start()
    {
        if (button == null || panel == null || string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("SpawnObjectButton: Some required components or variables are not assigned!");
            return;
        }

        button.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsConnected)
            {
                GameObject prefab = Resources.Load<GameObject>(prefabName);
                if (prefab != null)
                {
                    PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

                    // Disable the button for all clients
                    photonView.RPC("DisableButton", RpcTarget.All);

                    // Disable the UI for the local player
                    panel.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError("SpawnObjectButton: Prefab not found in Resources folder!");
                }
            }
        });
    }


    [PunRPC]
    void DisableButton()
    {
        button.interactable = false;
    }

}
