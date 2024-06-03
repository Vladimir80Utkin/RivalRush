using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Vector3 position = new Vector3(0f, 0f, -5f);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), position, Quaternion.identity);
    }
}
