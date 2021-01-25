using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    public Map[] stages;

    public static MapMove instance;


    [SerializeField]
    GameObject[] enemyPrefab;

    public GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void ChangeCamera(int stage)
    {
        stages[stage - 2].stageCamera.SetActive(false);
        stages[stage - 1].stageCamera.SetActive(true);
    }

    public void Respawn(int stage)
    {
        stages[stage - 1].RespawnPlayer();
    }

    public void EnemyRespawn(int stage)
    {
        enemies = new GameObject[stages[stage - 1].enemyRespawn.transform.childCount];
        for (int i = 0; i < stages[stage - 1].enemyRespawn.transform.childCount; i++)
        {
            enemies[i] = stages[stage - 1].enemyRespawn.transform.GetChild(i).gameObject;
        }


        for (int i = 0; i < enemies.Length; i++)
        {
            Transform[] enemyPosition = enemies[i].GetComponentsInChildren<Transform>();
            for(int j = 1; j < enemyPosition.Length;j++)
            {
                Instantiate(enemyPrefab[i], enemyPosition[j].position, Quaternion.identity);
            }
        }
        ColorManager.instance.enemyFind();
    }
}
