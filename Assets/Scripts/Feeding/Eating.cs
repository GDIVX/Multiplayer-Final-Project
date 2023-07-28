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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Player"))
        {
            //Compare the size of the two objects
            if (collision.gameObject.transform.localScale.x < transform.localScale.x)
            {
                //Destroy the smaller object
                PhotonView collisionPhotonView = collision.gameObject.GetComponent<PhotonView>();
                if (collisionPhotonView != null)
                {
                    photonView.RPC("DestroyObject", RpcTarget.All, collisionPhotonView.ViewID);
                }

                //Increase the size of the larger object
                float sizeIncrease = collision.gameObject.transform.localScale.x / transform.localScale.x;
                transform.localScale += new Vector3(sizeIncrease, sizeIncrease, 0);

                //Invoke the eating event
                OnEatingEvent?.Invoke(transform, sizeIncrease);
            }
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
