using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BulletCollisionHandler : MonoBehaviourPun
{
    public int damage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        // Проверяем, столкнулась ли пуля с игроком
        if (collision.gameObject.CompareTag("PlayerController"))
        {
            // Получаем компонент PhotonView цели столкновения
            PhotonView targetPV = collision.gameObject.GetComponent<PhotonView>();

            // Проверяем, является ли объект, с которым столкнулась пуля, игроком и принадлежит ли он текущему игроку
            if (targetPV != null && targetPV.IsMine)
            {
                // Получаем компонент PlayerHealth цели столкновения
                PlayerHealth targetHealth = collision.gameObject.GetComponent<PlayerHealth>();

                // Если компонент PlayerHealth доступен, применяем урон к игроку
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }
            }
        }

        // Уничтожаем пулю при столкновении с любым объектом
        Destroy(gameObject);
    }
}
