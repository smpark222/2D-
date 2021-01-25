using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class ColorManager : MonoBehaviour
{
    public bool isStop;
    public bool speedAttack;

    public static ColorManager instance;
    

    GameObject rangeSprite;
    Vector2 mPos;
    public Transform rangePos;
    Transform playerPos;

    public Coroutine routine;

    public LayerMask layerMask;
    public LayerMask layerMask2;

    public GameObject Point;
    public LineRenderer line;

    public Tilemap[] Map;
    SpriteRenderer[] ghostSprites;


    Color curPlayerColor;

    Color[] curBackGroundColor;
    Color[] curEnemiesColor;
    Color[] curMapColor;
    Color[] curDownJumpColor;
    Color[] curDownJumpWallColor;



    GameObject player;
    public GameObject[] enemies;
    public GameObject[] backGround;
    GameObject Range;

    public GameObject[] downJump;
    public GameObject[] downJumpWall;

    public AudioSource audioSource;
    Coroutine audioRoutine;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player = GameObject.Find("Player");
        playerPos = player.GetComponent<Transform>();

        ghostSprites = GameObject.Find("GhostManager").GetComponentsInChildren<SpriteRenderer>();

        Range = GameObject.Find("Range");
        rangeSprite = GameObject.Find("RangeSprite");

        downJump = GameObject.FindGameObjectsWithTag("Ground");
        downJumpWall = GameObject.FindGameObjectsWithTag("DownJumpWall");

        curMapColor = new Color[Map.Length];
        curDownJumpColor = new Color[downJump.Length];
        curDownJumpWallColor = new Color[downJumpWall.Length];
        curBackGroundColor = new Color[backGround.Length];

        curPlayerColor = player.GetComponent<SpriteRenderer>().color;

        for (int i = 0; i < backGround.Length; i++)
        {
            curBackGroundColor[i] = backGround[i].GetComponent<SpriteRenderer>().color;
        }


        for(int i = 0; i < Map.Length;i++)
        {
            //curMapColor[i] = Map[i].color;
            Map[i].color = new Color(0f, 0f, 0f, 0f);
        }
        for (int i = 0; i < downJump.Length; i++)
        {
            if(downJump[i].name != "Ground")
            {
                curDownJumpColor[i] = downJump[i].GetComponent<SpriteRenderer>().color;
            }
        }
        for (int i = 0; i < downJumpWall.Length; i++)
        {
            curDownJumpWallColor[i] = downJumpWall[i].GetComponent<SpriteRenderer>().color;
        }
    }
    public void enemyMissing()
    {
        enemies = new GameObject[0];
    }

    public void enemyFind()
    {
        GameObject[] backenemies = GameObject.FindGameObjectsWithTag("Enemy");

        enemies = new GameObject[backenemies.Length];

        enemies = backenemies;
        curEnemiesColor = new Color[enemies.Length];

        curEnemiesColor = new Color[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            curEnemiesColor[i] = enemies[i].GetComponent<SpriteRenderer>().color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        isStop = player.GetComponent<PlayerController>().timeStop;

        if (isStop)
        {
            Time.timeScale = 0.07f;

            if(audioRoutine != null)
            {
                StopCoroutine(audioRoutine);
            }
            audioRoutine = StartCoroutine(AudioStop(0.4f));

            for (int i = 0; i < ghostSprites.Length;i++)
            {
                ghostSprites[i].sortingOrder = 3;
            }
            if (Input.GetMouseButton(1) &&player.GetComponent<PlayerController>().dragon)
            {
                mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


                Vector2 dir = mPos - (Vector2)playerPos.position;
                Vector2 dirNo = dir.normalized;

                print(mPos);

                line.enabled = true;
                Point.SetActive(true);
                Range.SetActive(true);
                rangeSprite.SetActive(true);

                RaycastHit2D hit = Physics2D.Raycast(playerPos.position, dirNo, 7f, layerMask);

                
                rangePos = rangeSprite.GetComponent<Transform>();

                if (hit)
                {
                    if(Vector2.Distance(playerPos.position,mPos) > Vector2.Distance(playerPos.position, hit.point))
                    {
                       Vector2 back = (hit.point - (Vector2)playerPos.position).normalized;
                       rangePos.position =  hit.point - back;
                    }
                    else
                    {
                        rangePos.position = new Vector2(
                            playerPos.position.x + Vector2.ClampMagnitude(mPos - (Vector2)playerPos.position, 7f).x,
                            playerPos.position.y + Vector2.ClampMagnitude(mPos - (Vector2)playerPos.position, 7f).y);
                    }
                }
                else
                {
                    rangePos.position = new Vector2(
                        playerPos.position.x + Vector2.ClampMagnitude(mPos - (Vector2)playerPos.position, 7f).x,
                        playerPos.position.y + Vector2.ClampMagnitude(mPos - (Vector2)playerPos.position, 7f).y);
                }
                rangePos.localScale = new Vector2(Mathf.Sign(dirNo.x) * playerPos.localScale.x, 1f);

                Point.GetComponent<Transform>().position = rangePos.position;
                line.SetPosition(0, playerPos.position);
                line.SetPosition(1, rangePos.position);
            }

            if(Input.GetMouseButtonUp(1) && player.GetComponent<PlayerController>().dragon)
            {
                player.GetComponent<PlayerController>().timeStop = false;
                

                Point.SetActive(false);
                Range.SetActive(false);
                rangeSprite.SetActive(false);
                line.enabled = false;
                if(!speedAttack)
                    routine = StartCoroutine(SpeedAttack());
            }

            BlackScreen();
        }
        else
        {
            if (audioRoutine != null)
            {
                StopCoroutine(audioRoutine);
            }
            audioRoutine = StartCoroutine(AudioStop(1f));

            Time.timeScale = 1f;
            Point.SetActive(false);
            Range.SetActive(false);
            rangeSprite.SetActive(false);
            line.enabled = false;

            for (int i = 0; i < ghostSprites.Length; i++)
            {
                ghostSprites[i].sortingOrder = 1;
            }

            NormalScreen();
        }
    }

    IEnumerator AudioStop(float target)
    {
        audioSource.pitch = Mathf.Lerp(audioSource.pitch, target, Time.deltaTime * 10f);
        for(int i = 0; i < AudioManager.instance.audioSources.Length;i++)
        {
            AudioManager.instance.audioSources[i].audioSource.pitch =
                Mathf.Lerp(AudioManager.instance.audioSources[i].audioSource.pitch, target, Time.deltaTime * 10f);

        }
        yield return new WaitForFixedUpdate();
    }

    IEnumerator SpeedAttack()
    {
        Vector2 target = rangePos.position;

        StartCoroutine(player.GetComponent<PlayerController>().Attack());
        speedAttack = true;

        float distance;
        do
        {
            RaycastHit2D hit = Physics2D.Raycast(playerPos.position, target - (Vector2)playerPos.position, 1.5f, layerMask);
            RaycastHit2D hit2 = Physics2D.Raycast(playerPos.position, target - (Vector2)playerPos.position, 1.5f, layerMask2);
            float time = 0f;
            time += Time.deltaTime;

            if (!hit && !hit2)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                distance = Vector2.Distance(playerPos.position, target);
                playerPos.position = Vector2.MoveTowards(playerPos.position, target, Time.deltaTime * 85f);
                yield return new WaitForSeconds(0.001f);
            }
            else
            {
                break;
            }
            print(time);
            if (time > 0.4f)
                break;

        } while (distance > 0.1f);

        speedAttack = false;

    }

    public void BlackScreen()
    {
        //player.GetComponent<SpriteRenderer>().color = new Color(0.15f, 0.15f, 0.15f);
        for (int i = 0; i < backGround.Length; i++)
        {
            backGround[i].GetComponent<SpriteRenderer>().color = new Color(0.15f, 0.15f, 0.15f);
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
        }
        //for (int i = 0; i < Map.Length; i++)
        //{
        //    Map[i].color = new Color(0.15f,0.15f,0.15f);
        //}
        for (int i = 0; i < downJump.Length; i++)
        {
            if (downJump[i].name != "Ground")
            {
                downJump[i].GetComponent<SpriteRenderer>().color = new Color(0.15f  , 0.15f, 0.15f); ;
            }
        }
        for (int i = 0; i < downJumpWall.Length; i++)
        {
            downJumpWall[i].GetComponent<SpriteRenderer>().color = new Color(0.15f, 0.15f, 0.15f); ;
        }
    }

    public void NormalScreen()
    {
        player.GetComponent<SpriteRenderer>().color = curPlayerColor;

        for (int i = 0; i < backGround.Length; i++)
        {
            backGround[i].GetComponent<SpriteRenderer>().color = curBackGroundColor[i];
        }
        
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<SpriteRenderer>().color = curEnemiesColor[i];
        }
        //for (int i = 0; i < Map.Length; i++)
        //{
        //    Map[i].color = curMapColor[i];
        //}

        for (int i = 0; i < downJump.Length; i++)
        {
            if (downJump[i].name != "Ground")
            {
                 downJump[i].GetComponent<SpriteRenderer>().color = curDownJumpColor[i];
            }
        }

        for (int i = 0; i < downJumpWall.Length; i++)
        {
            downJumpWall[i].GetComponent<SpriteRenderer>().color = curDownJumpWallColor[i];
        }
    }
}
