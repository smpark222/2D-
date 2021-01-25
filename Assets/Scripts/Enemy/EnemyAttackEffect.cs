using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackEffect : MonoBehaviour
{
    Collider2D hit;
    BoxCollider2D box;
    public LayerMask layerMask;

    Transform playerPos;
    public Rigidbody2D enemyRigid;
    public Transform enemyPos;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.333f);
        playerPos = GameObject.Find("Player").GetComponent<Transform>();

        enemyPos = GetComponentInParent<Transform>();
        box = this.GetComponent<BoxCollider2D>();
        StartCoroutine(BoxEnable());
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.OverlapBox(new Vector2(this.transform.position.x + 0.01f, this.transform.position.y - 0.05f), new Vector2(0.66f, 0.39f), 0, layerMask);
        
    }

    IEnumerator BoxEnable()
    {
        yield return new WaitForSeconds(0.07f);
        box.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "playerHitBox")
        {
            if (hit)
            {
                Vector2 dir = this.transform.position - playerPos.position;
                enemyRigid.AddForce(new Vector2(dir.x * 150f, 0f));
                Destroy(gameObject);

            }
            else
            {
                StartCoroutine(Parry(collision));
            }
        }
        if(collision.tag == "AttackEffect")
        {
            Vector2 dir = this.transform.position - playerPos.position;
            enemyRigid.AddForce(new Vector2(dir.x * 150f, 0f));
            Destroy(gameObject);
        }
    }

    IEnumerator Parry(Collider2D collision)
    {
        yield return new WaitForSeconds(0.15f);
        if (!hit)
        {
            Rigidbody2D playerRigid = GameObject.Find("Player").GetComponentInParent<Rigidbody2D>();
            Transform playerPos = collision.gameObject.GetComponentInParent<Transform>();

            Vector2 dir = (playerPos.position - enemyPos.position).normalized;

            playerRigid.AddForce(new Vector2(dir.x * 1500, dir.y * 800));
            collision.gameObject.GetComponentInParent<PlayerController>().isDead = true;
        }
        else
        {
            Vector2 dir = this.transform.position - playerPos.position;
            enemyRigid.AddForce(new Vector2(dir.x * 150f, 0f));
            Destroy(gameObject);
        }
    }

}
