using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

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
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, Quaternion.identity, 0, new object[]{ PV.ViewID });
    }

    [PunRPC]
    public void Die(int viewID)
    {
        PhotonView playerPV = PhotonView.Find(viewID);
        if (playerPV != null && playerPV.IsMine)
        {
            if (playerPV.gameObject != null)
            {
                PhotonNetwork.Destroy(playerPV.gameObject);
            }
            CreateController();
        }
    }

    public void CallDie(int viewID)
    {
        PV.RPC("Die", RpcTarget.All, viewID);
    }
    
}
