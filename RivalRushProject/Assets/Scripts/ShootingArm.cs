using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingArm : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shotPoint;

    private float timeBtwShots;
    [SerializeField] private float startTimeBtwShots;

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f,0f, rotZ + offset);

        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(bullet, shotPoint.position, transform.rotation);
                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }
}
