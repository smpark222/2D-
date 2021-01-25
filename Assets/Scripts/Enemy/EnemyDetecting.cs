using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetecting : MonoBehaviour
{
    PolygonCollider2D PC;
    Transform playerPos;

    [SerializeField]
    LayerMask playerLayer;
    [SerializeField]
    LayerMask wallLayer;

    EnemyMove enemyMove;
    EnemyDead enemyDead;

    // Start is called before the first frame update
    void Start()
    {
        PC = GetComponent<PolygonCollider2D>();
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        enemyMove = GetComponentInParent<EnemyMove>();
        enemyDead = GetComponentInParent<EnemyDead>();
        LightSide();
        //PC.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyDead.isDead)
        {
            PC.enabled = false;
        }
        else
        {
            PC.enabled = true;
        }
    }

    public void DarkSide()
    {
        PC.SetPath(0, new[] { new Vector2(0f, 0f), new Vector2(8f, 1.5f), new Vector2(9f, 0.5f), new Vector2(9f, -0.5f), new Vector2(8f, -1.5f) });
    }

    public void LightSide()
    {
        PC.SetPath(0, new[] { new Vector2(0f, 0f), new Vector2(16f, 4f), new Vector2(18f, 1.5f), new Vector2(18f, -1.5f), new Vector2(16f, -4f) });
    }
    


    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, playerPos.position - this.transform.position, 15f, wallLayer);
            if (!hit)
            {
                enemyMove.detecting = true;
            }
        }
    }


}
