using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   [SerializeField] private float speed;
   [SerializeField] private float lifeTime;
   [SerializeField] private float distance;
   [SerializeField] private int damage;

   private void Update()
   {
      RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance);
      if (hitInfo.collider.CompareTag("Player"))
      {
         
      }
   }
}
