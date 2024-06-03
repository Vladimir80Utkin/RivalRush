using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShootingArm : MonoBehaviourPun
{
    [SerializeField] private float offset;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shotPoint;

    private float timeBtwShots;
    [SerializeField] private float startTimeBtwShots;

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleAiming();
            HandleShooting();
        }
    }

    private void HandleAiming()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }

    private void HandleShooting()
    {
        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                photonView.RPC("Shoot", RpcTarget.All, shotPoint.position, transform.rotation);
                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }

    [PunRPC]
    private void Shoot(Vector3 position, Quaternion rotation)
    {
        Instantiate(bullet, position, rotation);
    }
}
