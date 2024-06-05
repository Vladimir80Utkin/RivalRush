using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Coin : MonoBehaviourPun
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerController"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.AddCoin(1);
                    photonView.RPC("DestroyCoin", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    void DestroyCoin()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
