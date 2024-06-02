using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CoinSpawner : MonoBehaviourPun
{
    public GameObject coinPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    private float nextSpawnTime;

    private void Start()
    {
        if (coinPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Coin prefab or spawn points are missing!");
            enabled = false; // Отключаем этот скрипт, если не удается инициализировать настройки
            return;
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnCoin();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnCoin()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Vector3 spawnPosition = spawnPoint.position;

            if (Resources.Load<GameObject>("PhotonPrefabs/" + coinPrefab.name) == null)
            {
                Debug.LogError("Prefab not found in Resources/PhotonPrefabs folder: " + coinPrefab.name);
                return;
            }

            PhotonNetwork.Instantiate("PhotonPrefabs/" + coinPrefab.name, spawnPosition, Quaternion.identity);
        }
    }
}
