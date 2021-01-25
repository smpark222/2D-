using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunter_JumpAttack : MonoBehaviour
{
    HeadHunter headHunter;
    Rigidbody2D headHunterRigid;
    [SerializeField]
    Transform headHunterPos;

    BossAniManager bossAniManager;

    public bool isJumpAttack = false;
    bool Jump;
    bool Attack;
    bool stop = false;


    Coroutine routine;

    [SerializeField]
    private GameObject BulletPrefab;

    private void Awake()
    {
        headHunter = GetComponentInParent<HeadHunter>();
        headHunterRigid = GetComponentInParent<Rigidbody2D>();
        bossAniManager = GetComponentInParent<BossAniManager>();
    }

    private void Update()
    {
        if (isJumpAttack && !headHunter.isDead && !headHunter.isHurt)
        {
            if (Jump)
            {
                headHunterRigid.AddForce(new Vector2(400f * -headHunterPos.localScale.x, 800f));
                Jump = false;
                bossAniManager.Jump();

            }
            else
            {
                if (headHunter.isWall)
                {
                    bossAniManager.Wall(true);
                    if (stop)
                        headHunterRigid.velocity = Vector2.zero;

                    if (Attack)
                    {
                        stop = true;
                        StartCoroutine(WallJump());

                        Attack = false;

                    }
                }
                else
                {
                    bossAniManager.Wall(false);

                }


            }
            if (!Attack)
            {
                if (headHunter.isGrounded)
                {
                    JumpAttackGround();
                }
            }
        }
    }

    public void JumpAttack()
    {
        isJumpAttack = true;
        Jump = true;
        Attack = true;
    }

    public void JumpAttackGround()
    {
        bossAniManager.WallJumpReset();
        bossAniManager.JumpReset();
        if(!headHunter.isHurt)
            headHunter.isAttacking = false;
        isJumpAttack = false;
        Jump = false;
        Attack = false;
    }

    IEnumerator WallJump()
    {
        yield return new WaitForSeconds(0.3f);
        stop = false;
        headHunterRigid.AddForce(new Vector2(800f * headHunterPos.localScale.x, 600f));
        bossAniManager.Wall(false);
        bossAniManager.WallJump();
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(WallJumpShot());
    }

    IEnumerator WallJumpShot()
    {
        for (int i = 0; i < 17; i++)
        {
            GameObject go = Instantiate(BulletPrefab, headHunterPos.position, Quaternion.identity);
            if (headHunterPos.localScale.x > 0)
            {
                go.GetComponent<Bullet>().target = new Vector2((go.transform.position.x + Mathf.Cos(-Mathf.Deg2Rad * 10 * i)),
                                go.transform.position.y + Mathf.Sin(-Mathf.Deg2Rad * 10 * i));
            }
            else
            {
                go.GetComponent<Bullet>().target = new Vector2((go.transform.position.x + Mathf.Cos(-Mathf.Deg2Rad * (180 -(10 * i)))),
                                go.transform.position.y + Mathf.Sin(-Mathf.Deg2Rad * (180-(10 * i))));
            }
            yield return new WaitForSeconds(0.012f);
        }
    }
}

