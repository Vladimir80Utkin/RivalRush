using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, quaternion.identity);
        }
    }
    /*public void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1)
        {
            // Если текущий игрок - хост, передаем хостство другому игроку
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!PhotonNetwork.PlayerList[i].IsMasterClient)
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[i]);
                    break;
                }
            }
        }

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0); // Загружаем сцену с индексом 0
    }*/
}
