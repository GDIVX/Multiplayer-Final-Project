using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Eating : MonoBehaviourPunCallbacks
{
    [SerializeField] Collider2D eatingCollider;

    public event Action<Transform, float> OnEatingEvent;
    public event Action<Transform> OnEatenEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Player"))
        {
            //Compare the size of the two objects
            if (collision.gameObject.transform.localScale.x >= transform.localScale.x)
            {
                return;
            }
            //Destroy the smaller object
            PhotonView collisionPhotonView = collision.gameObject.GetComponent<PhotonView>();
            if (collisionPhotonView != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    GameManager.Instance.OnPlayerEaten(collision.transform.gameObject.GetPhotonView());
                }
                photonView.RPC("DestroyObject", RpcTarget.All, collisionPhotonView.ViewID);
            }

            //Increase the size of the larger object
            float sizeIncrease = collision.gameObject.transform.localScale.x / transform.localScale.x;
            transform.localScale += new Vector3(sizeIncrease, sizeIncrease, 0);

            //Invoke the eating event
            OnEatingEvent?.Invoke(transform, sizeIncrease);
        }
    }

    [PunRPC]
    void DestroyObject(int objectToDestroyID)
    {
        PhotonView photonViewToDestroy = PhotonView.Find(objectToDestroyID);
        if (photonViewToDestroy != null)
        {
            Destroy(photonViewToDestroy.gameObject);
        }
    }
}
