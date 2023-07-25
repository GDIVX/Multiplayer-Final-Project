using Assets.Scripts.PlayerControls;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IController, IPunObservable
{
    public Vector2 GetMovementVector()
    {
        Vector2 movementVector = Vector2.zero;

        // Only allow input if this client owns the player
        if (photonView.IsMine)
        {
            movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        return movementVector;
    }

    // Implement IPunObservable interface
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(GetMovementVector());
        }
        else
        {
            // Network player, receive data
            Vector2 movementVector = (Vector2)stream.ReceiveNext();
        }
    }
}
