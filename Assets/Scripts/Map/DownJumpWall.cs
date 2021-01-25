using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownJumpWall : MonoBehaviour
{
    BoxCollider2D wallBox;
    Rigidbody2D wallRigid;
    Rigidbody2D[] piecesRigid;
    Transform[] piecestransform;

    public GameObject[] piece;
    public Transform backPos;
    public Transform frontPos;

    private float pieceNum = 30f;

    // Start is called before the first frame update
    void Start()
    {
        wallBox = GetComponent<BoxCollider2D>();
        

        for (int i = 1; i < pieceNum; i++)
        {
            Instantiate(piece[Random.Range(0, 3)],
                new Vector2(backPos.position.x + (frontPos.position.x - backPos.position.x) * i / pieceNum, this.transform.position.y),
                Quaternion.Euler(0, 0, Random.Range(0, 360f)), this.transform);
        }

        piecesRigid = this.GetComponentsInChildren<Rigidbody2D>();
        piecestransform = this.GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Player").GetComponent<TimeBody>().isRewinding)
        {
            if(piecestransform[(int)pieceNum-1].position.y - this.transform.position.y < 0.1f)
            {
                GetComponent<SpriteRenderer>().enabled = true;

            }
            wallBox.enabled = true;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "AttackEffect")
        {
            wallBox.enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;

            for (int i = 1; i <  piecesRigid.Length;i++)
            {
                piecesRigid[i].bodyType = RigidbodyType2D.Dynamic;
                piecestransform[i].localScale = new Vector2(2.5f, 1.5f);

                piecesRigid[i].AddForce(new Vector2(Random.Range(-280f, 280f), Random.Range(280f, 480f)));
            }
        }
    }

}
