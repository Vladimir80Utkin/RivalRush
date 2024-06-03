using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerCamera : MonoBehaviourPun
{
    public Vector3 offset; // Смещение камеры относительно игрока

    private Transform playerTransform;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        playerTransform = transform.parent;

        // Инициализируем смещение, если оно не задано в инспекторе
        if (offset == Vector3.zero)
        {
            offset = new Vector3(0, 0, -1);
        }
    }

    /*private void Update()
    {
        if (playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position + offset;
            newPosition.z = -1; // Фиксируем позицию Z камеры
            transform.position = newPosition;
        }
    }*/
}
