using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedRange : MonoBehaviour
{
    Rigidbody2D playerRigid;

    [SerializeField]
    CircleCollider2D CC;
    // Start is called before the first frame update
    void Start()
    {
        playerRigid = GetComponentInParent<Rigidbody2D>();
    }

    public void Detecting(float velocity)
    {
        CC.radius = velocity / 2f;

        CC.enabled = true;
    }

    public void StopDetecting()
    {
        CC.enabled = true;

        CC.radius = 0.01f;
    }

    public void SlowRunDetecting()
    {
        CC.enabled = false;
    }

    public void AttackDetecting()
    {
        CC.enabled = true;
        CC.radius = 13f;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyMove>().detecting = true;
        }
    }



}
