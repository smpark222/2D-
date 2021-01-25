using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunter : MonoBehaviour
{
    Rigidbody2D bossRigid;
    [SerializeField]
    HeadHunter_JumpAttack attack;
    [SerializeField]
    HeadHunter_Aim laserAttack;

    [SerializeField]
    LayerMask GroundLayer;
    [SerializeField]
    LayerMask WallLayer;
    [SerializeField]
    Transform CheckPos;
    [SerializeField]
    public BossAniManager bossAni;

    Transform playerPos;


    float distanceToPlayer;
    float distanceToWall;

    public bool isGrounded { get; private set; }

    public bool isWall { get; private set; }

    public float bossHp = 3f;

    public bool isAttacking = false;
    public bool isDead = false;
    bool onetime = true;

    TimeBody timeBody;
    public bool isHurt = false;
    // Start is called before the first frame update
    void Start()
    {
        bossRigid = GetComponent<Rigidbody2D>();
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        bossAni = GetComponent<BossAniManager>();
        timeBody = GetComponent<TimeBody>();
    }

    // Update is called once per frame
    void Update()
    {
        ColCheck();
        DistanceCheck();

        if (!timeBody.isRewinding)
        {
            if (!isDead)
            {
                if (!isHurt)
                {
                    if (!isAttacking)
                    {
                        transform.localScale = new Vector2(Mathf.Sign(playerPos.position.x - transform.position.x), 1f);


                        if (distanceToPlayer < 3f && distanceToWall < 5f)
                        {
                            isAttacking = true;
                            bossAni.JumpAttack();
                        }
                        else
                        {
                            isAttacking = true;
                            bossAni.AimAttack();
                        }
                    }
                }
                else
                {
                    isAttacking = false;
                }


            }
        }
        else
        {
            bossHp = 3f;
            onetime = true;
            bossAni.Death(false);
            isDead = false;
            isAttacking = false;
        }
    }

    void DistanceCheck()
    {
        distanceToPlayer = Vector2.Distance(transform.position, playerPos.position);

        RaycastHit2D rightWall = Physics2D.Raycast(this.transform.position, Vector2.right,30f, WallLayer);
        RaycastHit2D leftWall = Physics2D.Raycast(this.transform.position, Vector2.left,30f, WallLayer);

        if (rightWall.collider != null && leftWall.collider != null)
        {
            if (rightWall.point.x - transform.position.x < transform.position.x - leftWall.point.x )
            {
                distanceToWall = rightWall.point.x - transform.position.x;
            }
            else
            {
                distanceToWall = transform.position.x - leftWall.point.x;
            }
        }
    }


    void ColCheck()
    {
        GroundCheck();
        if (!isGrounded) WallCheck();
        else isWall = false;
    }


    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(CheckPos.position, 0.5f, GroundLayer);
        bossAni.Ground(isGrounded);
    }

    void WallCheck()
    {
        isWall = Physics2D.OverlapCircle(CheckPos.position, 0.7f, WallLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            attack.JumpAttackGround();
        }
    }


}
