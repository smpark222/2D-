using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 dir;
    Vector2 dirNo;
    Transform playerPos;

    float angle;
    public bool Team = false; // false :  플레이어가 피격당함   true : 적이 피격당함

    public GameObject hitEffect;
    BoxCollider2D box;
    TrailRenderer trail;

    public Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        trail = this.GetComponent<TrailRenderer>();
        dir = target - this.transform.position;
        dirNo = dir.normalized;

        angle = Mathf.Atan2(dirNo.y, dirNo.x) * Mathf.Rad2Deg;
       
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        box = GetComponent<BoxCollider2D>();
        Destroy(gameObject,15f);
        //transform.localScale = new Vector3(1f, 3f);

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * 15f);
        if(Team)
        {
            trail.startColor = Color.green;
            trail.endColor = Color.green;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "playerHitBox")
        {
            if (!Team)
            {
                box.enabled = false;

                Rigidbody2D playerRigid = GameObject.Find("Player").GetComponentInParent<Rigidbody2D>();
                Transform playerPos = collision.gameObject.GetComponentInParent<Transform>();

                playerRigid.AddForce(new Vector2(dirNo.x * 1500, dirNo.y * 800));
                collision.gameObject.GetComponentInParent<PlayerController>().isDead = true;
                Destroy(gameObject);
            }
        }
        if(collision.tag == "Enemy")
        {
            if(Team && !collision.gameObject.GetComponent<EnemyDead>().isDead)
            {
                box.enabled = false;

                CameraController.instance.ShakeScreen();
                AniManager.instance.TimeStop();

                Vector3 dir = collision.transform.position - playerPos.position;
                Vector3 dirNo = dir.normalized;

                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dirNo.x * 1500f, dirNo.y * 900f));

                GameObject go = Instantiate(hitEffect, collision.transform.position - dirNo * 40f, Quaternion.identity);

                go.GetComponent<HitEffect>().dir = dirNo;

                collision.gameObject.GetComponent<EnemyDead>().isDead = true;

                collision.gameObject.GetComponent<EnemyAniManager>().Death();
                Destroy(gameObject);

            }
        }
        if (collision.tag == "Ground" || collision.tag == "Wall" || collision.tag == "Map" || collision.tag == "DownJumpWall")
        {
            Destroy(gameObject);
        }
    }


}
