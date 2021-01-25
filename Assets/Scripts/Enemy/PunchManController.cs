using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchManController : EnemyController
{
    Enemy enemyInfo;

    EnemyMove enemyMove;
    Transform playerPos;

    // Start is called before the first frame update
    void Start()
    {

        enemyInfo = SetEnemy();

        enemyMove = this.GetComponent<EnemyMove>();
        playerPos = GameObject.Find("Player").GetComponent<Transform>();

        this.GetComponent<SpriteRenderer>().sprite = enemyInfo.sprite;
        this.GetComponent<Animator>().runtimeAnimatorController = enemyInfo.animator;

        int num = Random.Range(0, 1);
        if (num == 0)
        {
            this.transform.localScale = new Vector2(-1, 1);
        }
        else if (num == 1)
        {
            this.transform.localScale = new Vector2(1, 1);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (enemyMove.finding && !GetComponent<EnemyDead>().isDead && !GameObject.Find("Player").GetComponent<PlayerController>().restart)
        {
            IdentifyAttackRange(enemyInfo, enemyMove, playerPos, transform);
        }
    }
    
   
}
