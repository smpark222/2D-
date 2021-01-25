using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    Transform playerPos;
    public GameObject hitEffect;
    ColorManager colorManager;
    PlayerController player;

    Collider2D hit;
    public LayerMask layerMask;

    Rigidbody2D playerRigid;

    BoxCollider2D effectBox;
    public GameObject slashFx;
    public GameObject bulletReflect;


    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerRigid = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        effectBox = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = playerPos.position;
        hit = Physics2D.OverlapBox(new Vector2(this.transform.position.x + 0.91f, this.transform.position.y - 0.06f), new Vector2(2.021668f, 1.310854f), 0, layerMask);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boss")
        {
            if (!collision.gameObject.GetComponent<HeadHunter>().isDead)
            {
                Instantiate(slashFx, collision.transform.position, Quaternion.identity);
                effectBox.enabled = false;

                CameraController.instance.ShakeScreen();
                AniManager.instance.TimeStop();

                Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 dir = mPos - (Vector2)collision.transform.position;
                Vector3 dirNo = dir.normalized;

                //collision.gameObject.GetComponent<Rigidbody2D>().sharedMaterial = collision.gameObject.GetComponent<EnemyMove>().NoFriction;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dirNo.x * 1500f, dirNo.y * 900f));

                GameObject go = Instantiate(hitEffect, collision.transform.position - dirNo * 40f, Quaternion.identity);

                go.GetComponent<HitEffect>().dir = dirNo;

                HeadHunter headHunter = collision.gameObject.GetComponent<HeadHunter>();
                headHunter.bossHp -= 1;
                headHunter.isHurt = true;

                if (headHunter.bossHp <= 0)
                {
                    headHunter.isDead = true;
                }
                headHunter.bossAni.Hurt();

                if (headHunter.isDead)
                {
                    headHunter.bossAni.Death(true);
                }
                else
                {
                    collision.gameObject.GetComponent<HeadHunter_Sweep>().Sweep();
                }


            }
        }
        if (collision.tag == "Enemy")
        {
            if (!collision.gameObject.GetComponent<EnemyDead>().isDead)
            {
                CameraController.instance.ShakeScreen();
                AniManager.instance.TimeStop();

                if (hit)
                {
                    Instantiate(bulletReflect, collision.transform.position, Quaternion.identity);

                    //playerRigid.velocity = Vector2.zero;
                    playerRigid.AddForce(new Vector2(-transform.localScale.x * 500f, 0f));
                    effectBox.enabled = false;
                }
                else
                {
                    //if (!colorManager.speedAttack)
                    //this.GetComponent<BoxCollider2D>().enabled = false;
                    Instantiate(slashFx, collision.transform.position, Quaternion.identity);

                    Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 dir = mPos - (Vector2)collision.transform.position;
                    Vector3 dirNo = dir.normalized;

                    collision.gameObject.GetComponent<Rigidbody2D>().sharedMaterial = collision.gameObject.GetComponent<EnemyMove>().NoFriction;
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dirNo.x * 1500f, dirNo.y * 900f));

                    GameObject go = Instantiate(hitEffect, collision.transform.position - dirNo * 40f, Quaternion.identity);

                    go.GetComponent<HitEffect>().dir = dirNo;

                    collision.gameObject.GetComponent<EnemyDead>().isDead = true;

                    collision.gameObject.GetComponent<EnemyAniManager>().Death();
                }
                
            }
        }
        if(collision.tag == "EnemyAttackEffect")
        {
            CameraController.instance.ShakeScreen();
            AniManager.instance.TimeStop();

            //playerRigid.velocity = Vector2.zero;

            Instantiate(bulletReflect, collision.transform.position, Quaternion.identity);

            playerRigid.AddForce(new Vector2(-transform.localScale.x * 500f, 0f));
            effectBox.enabled = false;
        }

        if(collision.tag == "Bullet")
        {
            if (!collision.gameObject.GetComponent<Bullet>().Team)
            {
                AudioManager.instance.PlaySound(AudioType.SlashBullet, true);
                Instantiate(bulletReflect, collision.transform.position, Quaternion.identity);


                CameraController.instance.ShakeScreen();
                AniManager.instance.TimeStop();

                Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 dirNo = (mPos - (Vector2)playerPos.position).normalized;
                float angle = Mathf.Atan2(dirNo.y, dirNo.x) * Mathf.Rad2Deg;

                collision.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(0f, 0f, angle) ;


                collision.gameObject.GetComponent<Bullet>().Team = true;
                collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
