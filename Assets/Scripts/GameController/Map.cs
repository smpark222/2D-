using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
    public int stageNum;

    public GameObject stageMap;

    public GameObject stageCamera;

    public Transform RespawnPoint;

    public GameObject enemyRespawn;

    public void RespawnPlayer()
    {
        Transform playerPos = GameObject.Find("Player").GetComponent<Transform>();
        playerPos.position = RespawnPoint.position;
    }
}
