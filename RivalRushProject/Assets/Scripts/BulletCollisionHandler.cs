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

            // Проверяем, является ли объект, с которым столкнулась пуля, игроком
            if (targetPV != null)
            {
                // Вызываем RPC функцию для нанесения урона на всех клиентах
                targetPV.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }
        }

        // Уничтожаем пулю при столкновении с любым объектом
        Destroy(gameObject);
    }
}
