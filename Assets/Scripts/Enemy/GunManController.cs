using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManController : EnemyController
{
    public GameObject arm;
    Enemy enemyInfo;

    EnemyMove enemyMove;
    Transform playerPos;

    GameObject GunArm;

    SpriteRenderer[] armImage;

    Sprite[] curArmImage;
    Transform bulletTransform;

    // Start is called before the first frame update
    void Start()
    {
        armImage = new SpriteRenderer[3];
        curArmImage = new Sprite[3];
        enemyInfo = SetEnemy();

        enemyMove = this.GetComponent<EnemyMove>();
        playerPos = GameObject.Find("Player").GetComponent<Transform>();

        this.GetComponent<SpriteRenderer>().sprite = enemyInfo.sprite;
        this.GetComponent<Animator>().runtimeAnimatorController = enemyInfo.animator;
        GunArm = Instantiate(arm, new Vector2(this.transform.position.x-0.185f* (transform.localScale.x),this.transform.position.y+0.335f), 
            Quaternion.identity, this.transform);
        armImage = GunArm.GetComponentsInChildren<SpriteRenderer>();

        curArmImage[0] = armImage[0].sprite;
        curArmImage[1] = armImage[1].sprite;
        curArmImage[2] = armImage[2].sprite;
        bulletTransform = GunArm.transform.GetChild(2).transform;

        int num = Random.Range(0, 1);
        if(num == 0)
        {
            this.transform.localScale = new Vector2(-1, 1);
        }
        else if(num == 1)
        {
            this.transform.localScale = new Vector2(1, 1);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (enemyMove.finding && !GetComponent<EnemyDead>().isDead && !GameObject.Find("Player").GetComponent<PlayerController>().restart)
        {
            IdentifyAttackRange(enemyInfo, enemyMove, playerPos,bulletTransform);
        }
        if(GetComponent<EnemyAniManager>().GetAim() && !enemyMove.turn)
        {
            armImage[0].sprite = curArmImage[0];
            armImage[1].sprite = curArmImage[1];
            armImage[2].sprite = curArmImage[2];

        }
        else
        {
            armImage[0].sprite = null;
            armImage[1].sprite = null;
            armImage[2].sprite = null;

        }
        if(GetComponent<EnemyDead>().isDead)
        {
            armImage[0].sprite = null;
            armImage[1].sprite = null;
            armImage[2].sprite = null;
        }
    }
    
   
}
