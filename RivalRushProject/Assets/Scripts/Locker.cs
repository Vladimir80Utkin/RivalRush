using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Locker : MonoBehaviourPun
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetInAmmoPickupZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetInAmmoPickupZone(false);
        }
    }
}
