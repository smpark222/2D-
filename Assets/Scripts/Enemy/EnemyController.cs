using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyInfo
{

    [SerializeField]
    EnemyType enemyType;

    public EnemyData enemyData;
}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyInfo enemyDatas = new EnemyInfo();
    Coroutine routine;

    [SerializeField]
    private LayerMask playerLayerMask;
    public LayerMask PlayerLayerMask { get; set; }

    public GameObject gunSpark;
    public GameObject gunSmoke;
    bool isCoolTime = false;

    public Enemy SetEnemy()
    {
        Enemy enemy = new Enemy();
        enemy.EnemyType = enemyDatas.enemyData.EnemyType;
        GetComponent<EnemyMove>().enemyType = enemyDatas.enemyData.EnemyType;
        enemy.AttackRange = enemyDatas.enemyData.AttackRange;
        enemy.AttackPrefab = enemyDatas.enemyData.AttackPrefab;
        enemy.AttackCoolTime = enemyDatas.enemyData.AttackCoolTime;
        enemy.sprite = enemyDatas.enemyData.Sprite;
        enemy.animator = enemyDatas.enemyData.Animator;
        return enemy;
    }

    protected void IdentifyAttackRange(Enemy enemy, EnemyMove enemyMove, Transform playerPos,Transform bullet)
    {

        RaycastHit2D playerChase = Physics2D.Raycast(this.transform.position, playerPos.position - this.transform.position, 15f, playerLayerMask);
        if (playerChase)
        {
            if (playerChase.collider.gameObject.tag == "Player")
            {
                enemyMove.seePlayer = true;
                if (!isCoolTime)
                    enemyMove.detecting = true;


                if (Vector2.Distance(playerChase.point, this.transform.position) < enemy.AttackRange)
                {
                    AttackController(enemy, enemyMove,bullet);
                }
                else
                {
                    enemyMove.isAttacking = false;
                }
            }
            else
            {
                enemyMove.isAttacking = false;
            }
        }
        else
        {
            enemyMove.seePlayer = false;
        }
    }

    void AttackController(Enemy enemy, EnemyMove enemyMove,Transform bullet)
    {
        enemyMove.isAttacking = true;

        if (!isCoolTime)
        {
            isCoolTime = true;
            if (!GetComponent<EnemyDead>().isDead)
            {
                routine = StartCoroutine(Attack(enemy, enemyMove, bullet));
            }
            else
            {
                StopCoroutine(routine);
            }
        }
    }

    IEnumerator Attack(Enemy enemy, EnemyMove enemyMove,Transform bullet)
    {
        yield return new WaitForSeconds(enemy.AttackCoolTime / 6f);

        if (!enemyMove.turn)
        {
            this.GetComponent<EnemyAniManager>().Run(false);
            if (enemy.EnemyType == EnemyType.PunchMan || enemy.EnemyType == EnemyType.KnifeMan)
            {
                this.GetComponent<EnemyAniManager>().Attack();
                yield return new WaitForSeconds(enemy.AttackCoolTime / 6f);

                GameObject go = Instantiate(enemy.AttackPrefab, new Vector2(this.transform.position.x + 0.4f * transform.localScale.x, this.transform.position.y + 0.17f),
                                            Quaternion.identity, this.transform);

                go.GetComponent<EnemyAttackEffect>().enemyRigid = enemyMove.enemyRigid;

                yield return new WaitForSeconds(enemy.AttackCoolTime * 1 / 3f);
            }
            else if (enemy.EnemyType == EnemyType.GunMan || enemy.EnemyType == EnemyType.RifleMan)
            {

                this.GetComponent<EnemyAniManager>().Aim(true);

                yield return new WaitForSeconds(enemy.AttackCoolTime / 6f);

                GameObject go = Instantiate(enemy.AttackPrefab, new Vector2(this.transform.position.x + 0.4f, this.transform.position.y + 0.17f), Quaternion.identity);
                go.GetComponent<Bullet>().target = GameObject.Find("Player").GetComponent<Transform>().position;
                Instantiate(gunSmoke, bullet.position, Quaternion.identity,transform);
                Instantiate(gunSpark, bullet.position, Quaternion.identity,transform);
                yield return new WaitForSeconds(enemy.AttackCoolTime * 1 / 3f);

            }

        }

        yield return new WaitForSeconds(enemy.AttackCoolTime * 1 / 3f);
        isCoolTime = false;
    }
}
