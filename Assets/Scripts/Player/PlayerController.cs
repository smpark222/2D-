using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region
    Rigidbody2D myRigid;
    AniManager aniManager;
    Transform player;

    Coroutine routine;
    Vector2 moveSpeed;

    Vector3 mPos;

    Vector2 dir;

    static float maxSpeed;
    static float slowSpeed;

    //[SerializeField]
    //float MaxSpeed;
    //float SlowSpeed;

    AudioSource AttackSound;

    [SerializeField]
    private float maxSlopeAngle;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    [SerializeField]
    private DetectedRange detectedRange;

    [SerializeField]
    private EffectFX[] effectFX;
    [SerializeField]
    private Transform Bottompos;

    private bool isOnSlope;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float lastSlopeAngle;
    private bool canWalkOnSlope;
    Vector2 newVelocity;


    [SerializeField]
    Vector2 jumpForce;
    Vector2 colliderSize;

    [SerializeField]
    private float slopeCheckDistance;


    public BoxCollider2D BC { get; set; }
    float landVelocity = 0f;

    [SerializeField]
    private Transform hitBox;

    private bool evadeSpeed = false;
    public bool isGround = true;
    public bool isAttacking = false;
    bool canMove = true;
    public bool isEvading = false;
    bool canEvade = true;
    public bool timeStop = false;
    public bool isWall = false;
    public bool wallJump = false;
    bool isJumping = false;

    public bool isDead = false;
    public bool rewinding = false;
    public bool restart = false;
    public bool dragon = true;
    public bool replaying = false;

    public bool clear = false;

    public int stage = 1;

    public bool stageMove = false;
    public bool fade = false;
    bool MapCol = false;
    public bool isRecording = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        AttackSound = GetComponent<AudioSource>();
        player = this.GetComponent<Transform>();
        myRigid = this.GetComponent<Rigidbody2D>();

        BC = GameObject.FindGameObjectWithTag("playerHitBox").GetComponent<BoxCollider2D>();

        colliderSize = BC.size;

        moveSpeed = new Vector2(50f, 0f);
        jumpForce = new Vector2(0f, 700f);

        maxSpeed = 8f;
        slowSpeed = maxSpeed / 3f;

        aniManager = AniManager.instance;

        MapMove.instance.Respawn(stage);
        MapMove.instance.EnemyRespawn(stage);

        aniManager.FightStart();
        Fade.instance.AlphaFadeOut();

    }

    // Update is called once per frame
    void Update()
    {
        ControlBodyType();
        if (!restart && !isDead && !clear)
        {
            if (aniManager.PlayerAni.GetBool("Start"))
            {
                isRecording = true;
                TimeControl();
                InputHandler();
                if (!isGround)
                {
                    JumpFall();
                }
            }
            else
            {
                WalkControl();
            }
        }
        if (isDead)
        {
            timeStop = false;
            Time.timeScale = 1f;

            DeadControl();
        }
        hitBox.position = this.transform.position;

        if(clear && stageMove &&  stage != MapMove.instance.stages.Length)
        {
            StartCoroutine(StageMove());
        }
    }

    IEnumerator StageMove()
    {
        AudioManager.instance.PlaySound(AudioType.Prerun, false);

        stageMove = false;
        stage++;

        fade = true;

        Fade.instance.SquareFadeIn();

        while (fade)
        {
            yield return Yields.FixedUpdate;
        }

        MapMove.instance.ChangeCamera(stage);

        ColorManager.instance.enemyMissing();

        GameObject[] removeEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        int cnt = removeEnemy.Length;
        for(int i = 0;  i < cnt;i++)
        {
            Destroy(removeEnemy[i]);
        }


        yield return new WaitForSeconds(2f);
        fade = true;
        CameraController.instance.GrayToColor();

        Fade.instance.SquareFadeOut();

        while(fade)
        {
            yield return Yields.FixedUpdate;
        }

        MapMove.instance.Respawn(stage);
        
        MapMove.instance.EnemyRespawn(stage);
        isRecording = true;
        clear = false;
        yield return null;

    }

    void DeadControl()
    {
        if (!restart)
        {
            StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        aniManager.Dead();
        AudioManager.instance.PlaySound(AudioType.Die, true);
        AudioManager.instance.PlaySound(AudioType.Prerun, false);
        myRigid.sharedMaterial = noFriction;
        restart = true;
        BC.enabled = false;
        isDead = false;
        yield return new WaitForSeconds(0.2f);
        while (!isGround)
        {
            yield return null;
        }
        aniManager.DeadGround();

        yield return new WaitForSeconds(2f);
        aniManager.Replay();
        rewinding = true;
        restart = false;
        isDead = false;

    }

    void ControlBodyType()
    {
        if (GetComponent<TimeBody>().isRewinding)
        {
            myRigid.velocity = Vector2.zero;
            myRigid.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            myRigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void WalkControl()
    {
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            player.localScale = new Vector3(1f, 1f, 1f);
            aniManager.Run(true);
            ApplyMovement(slowSpeed);


        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            player.localScale = new Vector3(-1f, 1f, 1f);
            aniManager.Run(true);
            ApplyMovement(-slowSpeed);


        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            aniManager.Run(false);
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            aniManager.Run(false);
        }
    }

    void JumpFall()
    {
        if (myRigid.velocity.y > 0)
        {
            aniManager.Jump();
        }
        else if (myRigid.velocity.y < 0)
        {
            aniManager.Fall();
        }
    }

    void InputHandler()
    {
        SlopeCheck();

        MoveControl();
        //ApplyMovement();



        AttackControl();
        EvadeControl();
        Wall();
    }

    void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);
        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            isOnSlope = false;

        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

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
        /*if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }*/

        if (isOnSlope && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && canEvade)
        {
            myRigid.sharedMaterial = fullFriction;
        }
        else
        {
            myRigid.sharedMaterial = noFriction;
        }
    }

    void Wall()
    {
        aniManager.Wall(isWall);
    }

    void TimeControl()
    {
        if (!this.GetComponent<TimeBody>().isRewinding)
        {

            if (Input.GetKeyDown(KeyCode.LeftShift) || (Input.GetMouseButtonDown(1) && dragon))
            {
                Time.timeScale = 0.07f;
                timeStop = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || (Input.GetMouseButtonUp(1) && dragon))
            {
                Time.timeScale = 1f;
                timeStop = false;
            }
        }
    }

    void EvadeControl()
    {
        if (Input.GetKeyUp(KeyCode.S) && isGround && !isAttacking && !isEvading && canEvade)
        {
            BC.enabled = false;
            aniManager.Evade();
            isEvading = true;
            canEvade = false;
            evadeSpeed = true;
            StartCoroutine(Evade());
        }
        if (evadeSpeed)
        {
            ApplyMovement(maxSpeed * 2.3f * transform.localScale.x);
        }
    }

    IEnumerator Evade()
    {
        myRigid.velocity = Vector2.zero;
        AudioManager.instance.PlaySound(AudioType.Roll, true);


        yield return new WaitForSeconds(0.1f);
        evadeSpeed = false;

        yield return new WaitForSeconds(0.2f);
        isEvading = false;

        yield return new WaitForSeconds(0.1f);
        canEvade = true;
        yield return new WaitForSeconds(0.4f);
        BC.enabled = true;
    }

    private void ApplyMovement(float a)
    {
        if (isGround && !isJumping)
        {
            if (!isOnSlope) //if not on slope
            {
                newVelocity.Set(a, 0.0f);
                myRigid.velocity = newVelocity;
            }
            else //If on slope
            {
                newVelocity.Set(-a * slopeNormalPerp.x, -a * slopeNormalPerp.y);
                myRigid.velocity = newVelocity;
            }
        }
        else if (!isGround && !isWall) //If in air
        {
            if(MapCol)
            {

            }
            newVelocity.Set(a, myRigid.velocity.y);
            myRigid.velocity = newVelocity;
        }

    }

    void MoveControl()
    {
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && canMove && !isEvading && !wallJump)
        {
            player.localScale = new Vector3(1f, 1f, 1f);
            aniManager.Run(true);

            if (isGround)
                AudioManager.instance.PlaySound(AudioType.Prerun, true);
            else
                AudioManager.instance.PlaySound(AudioType.Prerun, false);

            float speed;
            if (Input.GetKey(KeyCode.LeftControl) && isGround)
            {
                speed = slowSpeed;
                aniManager.SlowRun(true);

            }
            else
            {
                speed = maxSpeed;
                aniManager.SlowRun(false);
            }

            
            ApplyMovement(speed);

        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && canMove && !isEvading && !wallJump)
        {
            player.localScale = new Vector3(-1f, 1f, 1f);
            aniManager.Run(true);

            if(isGround)
                AudioManager.instance.PlaySound(AudioType.Prerun, true);
            else
                AudioManager.instance.PlaySound(AudioType.Prerun, false);


            float speed;

            if (Input.GetKey(KeyCode.LeftControl) && isGround)
            {
                aniManager.SlowRun(true);
                speed = -slowSpeed;

            }
            else
            {
                aniManager.SlowRun(false);
                speed = -maxSpeed;


            }
            ApplyMovement(speed);

        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            AudioManager.instance.PlaySound(AudioType.Prerun, false);

            aniManager.SlowRun(false);
            aniManager.Run(false);
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            AudioManager.instance.PlaySound(AudioType.Prerun, false);

            aniManager.SlowRun(false);
            aniManager.Run(false);

        }


        if(isEvading)
        {
            AudioManager.instance.PlaySound(AudioType.Prerun, false);

        }

        aniManager.WallJump(wallJump);

        if (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.S))
        {
            if (isWall && !isGround)
            {
                if (routine != null)
                    StopCoroutine(routine);
                wallJump = true;
                routine = StartCoroutine(WallJump());

            }

            else if (isGround && !isAttacking && !isEvading)
            {
                isJumping = true;
                AudioManager.instance.PlaySound(AudioType.Jump, true);
                myRigid.velocity = new Vector2(myRigid.velocity.x, 0f);
                myRigid.AddForce(jumpForce);

                Vector2 JumpPos = new Vector2(transform.position.x, transform.position.y + 0.2f);

                GameObject go = Instantiate(effectFX[0].FX, JumpPos, Quaternion.identity);
                Destroy(go, 0.4f);
            }
        }

        if (myRigid.velocity.y < -13f)
        {
            aniManager.Landing(true);
        }

        landVelocity = myRigid.velocity.y;


    }

    IEnumerator WallJump()
    {
        AudioManager.instance.PlaySound(AudioType.WallKick, true);

        myRigid.velocity = new Vector2(myRigid.velocity.x, 0f);
        myRigid.AddForce(new Vector2(800f * transform.localScale.x * -1f, 720f));
        aniManager.WallJump(wallJump);
        this.transform.localScale = new Vector2(this.transform.localScale.x * -1f, this.transform.localScale.y);

        yield return new WaitForSeconds(0.3f);
        wallJump = false;

    }

    void AttackControl()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isEvading && !wallJump && (!Input.GetMouseButton(1) || !dragon))
        {
            StartCoroutine(Attack());
        }
    }

    public IEnumerator Attack()
    {
        isAttacking = true;
        canMove = false;
        aniManager.Landing(false);
        AudioManager.instance.PlaySound(AudioType.Attack, true);

        mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = mPos - this.transform.position;
        Vector2 dirNo = dir.normalized;
        myRigid.velocity = Vector2.zero;


        if (this.transform.position.y < mPos.y || !isGround)
        {
            myRigid.AddForce(dirNo * 480f);
        }
        else
        {
            myRigid.AddForce(dirNo * 240f);
        }


        if (this.transform.position.x > mPos.x)
        {
            this.transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector2(1f, 1f);
        }

        if (this.transform.position.y > mPos.y && isGround)
        {
            aniManager.GroundAttack(dirNo);
        }
        else
        {
            aniManager.JumpAttack(dirNo);
        }

        yield return new WaitForSeconds(0.3f);
        aniManager.effect.GetComponent<BoxCollider2D>().enabled = false;
        canMove = true;

        yield return new WaitForSeconds(0.27f);
        isAttacking = false;

        StopCoroutine(Attack());
    }

    IEnumerator landing()
    {

        yield return new WaitForSeconds(0.1f);
        aniManager.Landing(false);
    }

    IEnumerator Detecting()
    {
        if (landVelocity < 0)
            detectedRange.Detecting(-landVelocity);
        landVelocity = 0f;
        yield return new WaitForSeconds(0.1f);
        detectedRange.StopDetecting();
        StopCoroutine(Detecting());
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
            aniManager.Land();
            StartCoroutine(landing());
        }


        if (collision.gameObject.tag == "Wall")
        {
            if ((this.transform.localScale.x == 1f && Input.GetKey(KeyCode.D))
                || (this.transform.localScale.x == -1f && Input.GetKey(KeyCode.A)))
            {
                if (!isGround)
                {
                    myRigid.velocity = new Vector2(myRigid.velocity.x, myRigid.velocity.y * 0.7f);
                    isWall = true;
                }
                else
                {
                    isWall = false;
                }
            }
            else
            {
                isWall = false;
            }

        }
        if(collision.gameObject.tag == "Map")
        {
            MapCol = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            isJumping = false;

            Vector2 effectPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
            GameObject go = Instantiate(effectFX[1].FX, effectPos, Quaternion.identity);
            Destroy(go, 0.4f);

            StartCoroutine(Detecting());
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Stair")
        {
            isGround = false;
        }
        if (collision.gameObject.tag == "Wall")
        {
            isWall = false;
        }
        if (collision.gameObject.tag == "Map")
        {
            MapCol = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ClearPoint")
        {
            StartCoroutine(NextStage());
        }
    }

    IEnumerator NextStage()
    {
        clear = true;
        float time = 0f;
        while (time < 1.3f)
        {
            time += Time.deltaTime;
            ApplyMovement(maxSpeed * transform.localScale.x);
            yield return null;

        }
        replaying = true;
        yield return null;
    }
}
