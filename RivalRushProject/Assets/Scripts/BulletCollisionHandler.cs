using Photon.Pun;
using UnityEngine;

public class BulletCollisionHandler : MonoBehaviourPun
{
    public int damage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("PlayerController"))
        {
            PhotonView targetPV = collision.gameObject.GetComponent<PhotonView>();
            
            if (targetPV != null)
            {
                targetPV.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }
        }
        Destroy(gameObject);
    }
}
