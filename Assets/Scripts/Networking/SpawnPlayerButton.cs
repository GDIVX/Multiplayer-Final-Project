using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPlayerButton : MonoBehaviourPun
{
    [SerializeField] Button button;
    [SerializeField] Transform panel;
    [SerializeField, FilePath(ParentFolder = "Assets/Resources")] string prefabName;
    [SerializeField] float spawnRadius = 5f;

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

                    GameObject playerObj = PhotonNetwork.Instantiate(prefabName, Random.insideUnitCircle * spawnRadius, Quaternion.identity);
                    PhotonView playerPhotonView = playerObj.GetPhotonView();

                    if (playerPhotonView != null && playerPhotonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        // Disable the button for all clients
                        photonView.RPC("DisableButton", RpcTarget.All);

                        // Disable the UI for the local player
                        panel.gameObject.SetActive(false);

                        //add the player to the list of tracked players
                        photonView.RPC("AddActivePlayer", RpcTarget.MasterClient, playerPhotonView.ViewID);
                    }
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

    [PunRPC]
    void AddActivePlayer(int viewID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // Find the player's PhotonView
        PhotonView playerPhotonView = PhotonView.Find(viewID);

        // Add the player to the list of tracked players
        GameManager.Instance.AddActivePlayer(playerPhotonView);
    }

}
