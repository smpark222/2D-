using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    SpriteRenderer spriteColor;
    public bool destroy = false;
    CapsuleCollider2D box;


    // Start is called before the first frame update
    void Start()
    {
        spriteColor = GetComponent<SpriteRenderer>();
        StartCoroutine(LaserColor());
        box = GetComponent<CapsuleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (destroy)
        {
            StartCoroutine(DestroyLaser());
            destroy = false;
        }
    }
    IEnumerator LaserColor()
    {
        while (true)
        {
            spriteColor.color = new Color(1f, 1 / 2f, 0f);
            yield return new WaitForSeconds(0.05f);
            spriteColor.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator DestroyLaser()
    {
        while (transform.localScale.y > 0.01f)
        {
            transform.localScale = new Vector2(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0f, Time.deltaTime * 10f));
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "playerHitBox")
        {
            box.enabled = false;

            Rigidbody2D playerRigid = GameObject.Find("Player").GetComponentInParent<Rigidbody2D>();
            Transform playerPos = collision.gameObject.GetComponentInParent<Transform>();

            playerRigid.AddForce(new Vector2(Mathf.Sign(playerPos.position.x -transform.position.x) * 1500f, 500f));
            collision.gameObject.GetComponentInParent<PlayerController>().isDead = true;
        }
    }
}
