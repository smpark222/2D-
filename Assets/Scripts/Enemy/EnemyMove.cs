using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private LayerMask UpStairlayerMask;
    [SerializeField]
    private LayerMask DownStairlayerMask;
    [SerializeField]
    private LayerMask StairlayerMask;
    [SerializeField]
    private LayerMask waterlayerMask;
    [SerializeField]
    private LayerMask GroundwaterlayerMask;
    [SerializeField]
    private LayerMask playerLayerMask;

    [SerializeField]
    private EdgeCollider2D edge;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D NoFriction { get; }

    [SerializeField]
    private PhysicsMaterial2D fullFriction;
    public PhysicsMaterial2D FullFriction { get; }

    [SerializeField]
    private float EnemySpeed = 6f;
    [SerializeField]
    private float maxSlopeAngle;
    [SerializeField]
    private float groundCheckRadius;


    [SerializeField]
    private float slopeCheckDistance;


    public Rigidbody2D enemyRigid;
    BoxCollider2D bc;
    Transform playerPos;
    public Transform ePos;

    TimeBody timebody;

    EnemyAniManager enemyAni;

    Coroutine Downroutine;
    Coroutine Uproutine;

    Vector2 curPos;
    Vector2 pos;
    Vector2 dir;
    Vector2 newVelocity;
    Vector2 stairTarget;
    Vector2 colliderSize;
    Vector2 target;

    public EnemyType enemyType;

    public bool isOnSlope;
    public bool detecting = false;
    public bool finding = false;
    public bool isAttacking = false;
    public bool turn = false;
    public bool findPlayer = false;
    bool stairArrive = false;
    bool arrive = false;
    bool go = false;
    bool isGrounded;
    bool onetime = false;
    public bool seePlayer = false;


    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float lastSlopeAngle;
    float detectingtime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        enemyAni = this.GetComponent<EnemyAniManager>();
        enemyRigid = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();
        colliderSize = bc.size;
        stairArrive = false;
        playerPos = GameObject.Find("Player").GetComponent<Transform>();

        curPos = ePos.position;

        timebody = this.GetComponent<TimeBody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (timebody.isRewinding)
        {
            finding = false;
            detecting = false;
            isAttacking = false;
            onetime = false;
            enemyAni.Revive();
        }

        if (!this.GetComponent<EnemyDead>().isDead && !isAttacking && !GameObject.Find("Player").GetComponent<PlayerController>().restart 
            &&!GameObject.Find("Player").GetComponent<PlayerController>().clear)
        {
            if (enemyType == EnemyType.GunMan || enemyType == EnemyType.RifleMan)
            {
                enemyAni.Aim(false);
            }
            EnemyDetect();
            ControlBodyType();
            CheckGround();
            SlopeCheck();
            if (go && !arrive)
                CharacterMove();
        }
        if(isAttacking && !turn && !this.GetComponent<EnemyDead>().isDead &&
            !GameObject.Find("Player").GetComponent<PlayerController>().restart
            && !GameObject.Find("Player").GetComponent<PlayerController>().clear)
        {
            TurnToTarget();
        }
    }

    void TurnToTarget()
    {
        if (Mathf.Sign(playerPos.position.x - this.transform.position.x) != this.transform.localScale.x)
        {
            StartCoroutine(Turn());
            turn = true;
        }
    }

    void EnemyDetect()
    {
        if (detecting)
        {
            target = playerPos.position;

            findPlayer = true;
            finding = true;
            detecting = false;
            detectingtime = 0f;
        }

        if (finding)
        {
            detectingtime += Time.deltaTime;
            if (detectingtime > 5f)
            {
                finding = false;
            }
        }
        else
        {
            detectingtime = 0f;
            findPlayer = false;
        }

        if (findPlayer)
        {

            go = true;
            arrive = false;
            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.down, 100f, GroundwaterlayerMask);

            if (hit)
            {
                target = new Vector2(target.x, hit.point.y);
            }
            else
            {
                go = false;
                arrive = true;
            }
        }
        else if (!finding)
        {
            isAttacking = false;

            target = curPos;
            go = true;
            arrive = false;
            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.down, 100f, GroundwaterlayerMask);

            if (hit)
            {
                target = new Vector2(target.x, hit.point.y);
            }
            else
            {
                go = false;
                arrive = true;
            }
        }

    }

    void ControlBodyType()
    {
        if (GetComponent<TimeBody>().isRewinding)
        {
            enemyRigid.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            enemyRigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }



    void CharacterMove()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(ePos.position, Vector2.down, 0.2f, GroundwaterlayerMask);

        if (hit2D)
        {
            pos = hit2D.point;
            if (target.y + 0.001f < pos.y)
            {
                StopCoroutine(FindUpStairTarget());
                Uproutine = null;

                Downstair();
            }

            else if (target.y > pos.y + 0.001f)
            {

                StopCoroutine(FindDownStairTarget());
                Downroutine = null;
                UpStair();
            }
            else
            {
                if (Uproutine != null)
                {
                    StopCoroutine(Uproutine);
                    Uproutine = null;
                }
                if (Downroutine != null)
                {
                    StopCoroutine(Downroutine);
                    Downroutine = null;

                }
                SameFloor();
            }
        }

        if (Vector2.Distance(target, this.transform.position) < 1f)
        {
            findPlayer = false;
            arrive = true;
            go = false;
        }
        if (!arrive)
        {

            if (Mathf.Sign(dir.x) != this.transform.localScale.x)
            {
                if (!turn && !isAttacking)
                {
                    StartCoroutine(Turn());
                    turn = true;
                }
            }

            if (!turn && !isAttacking)
            {
                if (findPlayer)
                {
                    ApplyMovement(EnemySpeed * Mathf.Sign(dir.x));
                    enemyAni.Run(true);
                    enemyAni.Walk(false);
                }
                else
                {
                    ApplyMovement(EnemySpeed / 2 * Mathf.Sign(dir.x));
                    enemyAni.Run(false);
                    enemyAni.Walk(true);
                }
            }
        }

        else
        {
            enemyRigid.velocity = Vector2.zero;
            enemyAni.Run(false);
            enemyAni.Walk(false);
        }
    }

    IEnumerator Turn()
    {
        enemyAni.Run(false);
        enemyAni.Walk(false);

        if (!isAttacking)
        {
            transform.localScale = new Vector3(Mathf.Sign(playerPos.position.x - this.transform.position.x), 1f, 1f);


            enemyAni.Turn();
        }
        else if(enemyType == EnemyType.GunMan || enemyType == EnemyType.RifleMan)
        {
            enemyAni.Aim(false);
            transform.localScale = new Vector3(Mathf.Sign(playerPos.position.x - this.transform.position.x), 1f, 1f);
            enemyAni.Turn();
        }
        yield return new WaitForSeconds(0.66f);

        turn = false;
    }


    void FindStair(LayerMask layer)
    {


        RaycastHit2D RightHit = Physics2D.Raycast(groundCheck.position, Vector2.right, 100f, layer);
        RaycastHit2D LeftHit = Physics2D.Raycast(groundCheck.position, Vector2.left, 100f, layer);


        if (RightHit && LeftHit)
        {

            float distanceRight = Vector2.Distance(RightHit.point, this.transform.position);
            float distanceLeft = Vector2.Distance(LeftHit.point, this.transform.position);
            if (distanceLeft > distanceRight)
            {
                stairTarget = RightHit.point;
            }
            else
            {
                stairTarget = LeftHit.point;
            }
        }

        else if (RightHit && !LeftHit)
        {

            stairTarget = RightHit.point;
        }

        else if (!RightHit && LeftHit)
        {

            stairTarget = LeftHit.point;
        }
        else
        {
        }

    }

    void Downstair()
    {
        bool isStairTarget = Physics2D.OverlapCircle(this.transform.position, 0.1f, DownStairlayerMask);
        bool isStair = Physics2D.OverlapCircle(new Vector2(ePos.position.x, ePos.position.y + 0.15f), 0.1f, StairlayerMask);

        RaycastHit2D DownHit = Physics2D.Raycast(this.transform.position, Vector2.down, 5f, layerMask);

        FindStair(DownStairlayerMask);

        if (!stairArrive && !isStair)
        {
            dir = stairTarget - (Vector2)this.transform.position;
        }



        if (isStair)
        {
            stairArrive = false;
            edge.enabled = true;
            //dir = stairTarget;
            // ApplyMovement(EnemySpeed * Mathf.Sign(dir.x)
            if (Downroutine == null)
            {
                Downroutine = StartCoroutine(FindDownStairTarget());
            }
        }
        else
        {

        }

        if (Vector2.Distance(stairTarget, this.transform.position) > 0.01f)
        {
            if (isStairTarget)
            {
                edge.enabled = false;
                stairArrive = true;

                if (Downroutine == null)
                {
                    Downroutine = StartCoroutine(FindDownStairTarget());
                }
            }
        }
    }

    IEnumerator FindDownStairTarget()
    {
        //Collider2D DownRightStairTarget = null;
        //Collider2D DownLeftStairTarget = null;


        //DownRightStairTarget = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(11f, -5f), new Vector2(22f, 10f), 0, UpStairlayerMask);
        //DownLeftStairTarget = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(-11f, -5f), new Vector2(22f, 10f), 0, UpStairlayerMask);
        RaycastHit2D ray1 = Physics2D.Raycast(this.transform.position, new Vector2(-0.2f, -0.1f), Mathf.Infinity, UpStairlayerMask);



        if (ray1)
        {
            dir = ray1.point - (Vector2)this.transform.position;
            stairTarget = ray1.point;
        }
        else
        {
            RaycastHit2D ray2 = Physics2D.Raycast(this.transform.position, new Vector2(0.2f, -0.1f), Mathf.Infinity, UpStairlayerMask);
            if (ray2)
            {
                dir = ray2.point - (Vector2)this.transform.position;
                stairTarget = ray2.point;
            }
        }

        yield return null;

        Downroutine = null;
        StopCoroutine(FindDownStairTarget());
    }

    void UpStair()
    {
        bool isStairTarget = Physics2D.OverlapCircle(groundCheck.position, 0.1f, UpStairlayerMask);
        bool isStair = Physics2D.OverlapCircle(ePos.position, 0.1f, StairlayerMask);

        FindStair(UpStairlayerMask);

        if (!stairArrive && !isStair)
        {
            dir = stairTarget - (Vector2)this.transform.position;
        }

        if (isStair)
        {
            stairArrive = false;
            edge.enabled = false;
            if (Uproutine == null)
            {
                Uproutine = StartCoroutine(FindUpStairTarget());
            }
        }

        else
        {
            edge.enabled = true;
        }

        if (Vector2.Distance(stairTarget, this.transform.position) > 0.01f)
        {
            if (isStairTarget)
            {
                edge.enabled = true;
                stairArrive = true;

                if (Uproutine == null)
                {

                    Uproutine = StartCoroutine(FindUpStairTarget());
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector2(-0.2f, 0.1f) * 100f);
        Gizmos.DrawRay(transform.position, new Vector2(0.2f, 0.1f) * 100f);


    }
    IEnumerator FindUpStairTarget()
    {


        RaycastHit2D ray1 = Physics2D.Raycast(this.transform.position, new Vector2(-0.2f, 0.1f), Mathf.Infinity, DownStairlayerMask);
        RaycastHit2D ray2 = Physics2D.Raycast(this.transform.position, new Vector2(0.2f, 0.1f), Mathf.Infinity, DownStairlayerMask);

        if (ray1 && ray2)
        {
            if (ray1.point.y > ray2.point.y)
            {
                dir = ray2.point - (Vector2)this.transform.position;
                stairTarget = ray2.point;
            }
            else
            {
                dir = ray1.point - (Vector2)this.transform.position;
                stairTarget = ray1.point;
            }
        }
        else if (ray1)
        {
            dir = ray1.point - (Vector2)this.transform.position;
            stairTarget = ray1.point;
        }
        else if (ray2)
        {
            dir = ray2.point - (Vector2)this.transform.position;
            stairTarget = ray2.point;
        }


        yield return null;

        Uproutine = null;
        StopCoroutine(FindUpStairTarget());
    }



    void SameFloor()
    {

        dir = target - (Vector2)this.transform.position;
    }


    void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(this.transform.position, Vector2.down, 5f, layerMask);

        LayerMask mask;

        if (hit2D)
            mask = layerMask;
        else
        {
            mask = whatIsGround;
        }

        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, mask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, mask);

        if (slopeHitFront)
        {
            isOnSlope = true;

        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
        }
        else
        {
            isOnSlope = false;

        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(this.transform.position, Vector2.down, 5f, layerMask);

        LayerMask mask;

        if (hit2D)
            mask = layerMask;
        else
        {
            mask = whatIsGround;
        }


        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, mask);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (isOnSlope && !go)
        {
            enemyRigid.sharedMaterial = fullFriction;
        }
        else
        {
            enemyRigid.sharedMaterial = noFriction;
        }
    }

    private void ApplyMovement(float a)
    {
        if (isGrounded)
        {
            if (!isOnSlope) //if not on slope
            {
                newVelocity.Set(a, 0.0f);
                enemyRigid.velocity = newVelocity;
            }
            else //If on slope
            {
                newVelocity.Set(-a * slopeNormalPerp.x, -a * slopeNormalPerp.y);
                enemyRigid.velocity = newVelocity;
            }
        }
        else
        {
            newVelocity.Set(a, enemyRigid.velocity.y);
            enemyRigid.velocity = newVelocity;
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<EnemyDead>().isDead)
        {
            if (collision.gameObject.tag == "Ground")
            {
                onetime = false;
                if (!onetime || Mathf.Abs(enemyRigid.velocity.x) < 0.2f)
                {
                    onetime = true;

                    enemyAni.Ground();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (GetComponent<EnemyDead>().isDead)
        {
            if (collision.gameObject.tag == "Ground")
            {
                if (Mathf.Abs(enemyRigid.velocity.x) < 0.01f)
                {
                    onetime = true;

                    enemyAni.Ground();
                }
            }
        }
    }

}
