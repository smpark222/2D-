using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : MonoBehaviour
{
    public ParticleSystem[] Blood;
    public GameObject[] BackBlood;
    BoxCollider2D enemyBox;
    Rigidbody2D enemyRigid;


    public bool isDead = false;
    public bool isGround = false;
    bool onetime = true;
    bool onetime1 = true;
    bool onetime2 = true;
    bool playerDead = true;

    // Start is called before the first frame update
    void Start()
    {
        enemyBox = this.GetComponent<BoxCollider2D>();
        enemyRigid = this.GetComponent<Rigidbody2D>();
        for(int i = 0; i < Blood.Length;i++)
        {
            Blood[i].Stop();
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        playerDead = this.GetComponent<TimeBody>().isRewinding;

        if (isDead)
        {
            if (onetime1)
            {
                StartCoroutine(DeadEffect());
                onetime1 = false;
            }

            if (isGround && onetime && Mathf.Abs(enemyRigid.velocity.x) < 0.1f)
            {
                //enemyRigid.bodyType = RigidbodyType2D.Static;
                    
                onetime = false;
            }
        }

        if(playerDead)
        {
            //enemyRigid.bodyType = RigidbodyType2D.Dynamic;
            enemyRigid.constraints = RigidbodyConstraints2D.None;
            enemyRigid.constraints = RigidbodyConstraints2D.FreezeRotation;

            isDead = false;
            onetime = true;
            onetime1 = true;
        }
    }
        
    IEnumerator DeadEffect()
    {
        for (int i = 0; i < Blood.Length; i++)
        {
            Blood[i].Play();
        }

        Instantiate(BackBlood[Random.Range(3, 7)], this.transform.position, Quaternion.identity,
            GameObject.Find("BloodEffect").GetComponent<Transform>());
        yield return new WaitForSeconds(0.075f);

        for (int i = 0; i < 40; i++)
        {
            Instantiate(BackBlood[Random.Range(0, 7)], this.transform.position, Quaternion.identity,
                GameObject.Find("BloodEffect").GetComponent<Transform>());
            yield return new WaitForSeconds(0.075f);
        }
        for (int i = 0; i < Blood.Length; i++)
        {
            Blood[i].Stop();
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            //enemyRigid.constraints = RigidbodyConstraints2D.FreezePositionY;
            isGround = true;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (isDead)
            {
                enemyRigid.velocity = new Vector2(enemyRigid.velocity.x * -0.4f, enemyRigid.velocity.y);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = false;
        }
    }

}
