using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct EffectFX
{
    public enum EffectName
    {
        Jump,
        Land
    }

    [SerializeField]
    EffectName effectName;

    public GameObject FX;
}


public class AniManager : MonoBehaviour
{
    public static AniManager instance;
    public Animator PlayerAni;
    bool isStart = false;
    

    public GameObject effect;
    public GameObject ghost;
    ColorManager colorManager;

   

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        PlayerAni = GetComponent<Animator>();
        effect = GameObject.Find("AttackEffect");
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();

        StartCoroutine(MakeGhost());
    }

    IEnumerator MakeGhost()
    {
        GameObject[] go = new GameObject[50];
        for (int i = 0; i < 50; i++)
        {
            go[i] = Instantiate(ghost, GameObject.Find("GhostManager").GetComponent<Transform>());
            //go[i].GetComponent<SpriteRenderer>().color = go[i].GetComponent<Ghost>().colors[i % go[i].GetComponent<Ghost>().colors.Length];
        }
        while (true)
        {
            TimeBody timebody = GameObject.Find("Player").GetComponent<TimeBody>();
            if (!timebody.isReplaying && !timebody.isRewinding)
            {
                for (int i = 0; i < 50; i++)
                {
                    StartCoroutine(go[i].GetComponent<Ghost>().PlayAni());

                    yield return new WaitForSeconds(0.00005f);

                }
            }
            else
            {
                yield return null;
            }
        }

    }

    public void FightStart()
    {
        isStart = !isStart;
        PlayerAni.SetBool("Start", isStart);

    }

    public void Dead()
    {
        PlayerAni.SetTrigger("Dead");
    }

    public void DeadGround()
    {
        PlayerAni.SetTrigger("DeadGround");
    }

    public void Replay()
    {
        PlayerAni.SetTrigger("Replay");
    }

    public void Run(bool run)
    {
        PlayerAni.SetBool("Run", run);
    }

    public void Jump()
    {
        PlayerAni.SetBool("Fall", false);
        
        PlayerAni.SetBool("Jump", true);
    }
    public void Fall()
    {
        PlayerAni.SetBool("Jump", false);
        PlayerAni.SetBool("Fall", true);
    }

    public void Land()
    {
        PlayerAni.SetBool("Jump", false);

        PlayerAni.SetBool("Fall", false);
    }

    public void Landing(bool landing)
    {
        PlayerAni.SetBool("Land", landing);
    }

    public void WallJump(bool wallJump)
    {
        PlayerAni.SetBool("WallJump", wallJump);
    }


    public void GroundAttack(Vector2 dir)
    {
        PlayerAni.SetTrigger("GroundAttack");

        effect.GetComponent<Animator>().SetTrigger("Start");


        //GameObject go = Instantiate(effect, this.transform.position, Quaternion.identity);
        EffectDirection(effect, dir, effect.transform.localScale);
        StartCoroutine(EffectBoxEnabled());
    }

    public void JumpAttack(Vector2 dir)
    {
        PlayerAni.SetTrigger("JumpAttack");
        effect.GetComponent<Animator>().SetTrigger("Start");
        EffectDirection(effect, dir, effect.transform.localScale);
        StartCoroutine(EffectBoxEnabled());
        //GameObject go = Instantiate(effect, this.transform.position, Quaternion.identity);
        // EffectDirection(go, dir);

    }

    IEnumerator EffectBoxEnabled()
    {
        yield return new WaitForSeconds(0.05f);
        effect.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void EffectDirection(GameObject go, Vector2 dir, Vector2 scale)
    {
        go.transform.localScale = new Vector2(scale.x, scale.y * Mathf.Sign(dir.x));

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SlowRun(bool slow)
    {
        PlayerAni.SetBool("SlowRun", slow);
    }

    public void Evade()
    {
        PlayerAni.SetTrigger("Evade");
    }

    public void TimeStop()
    {
        StartCoroutine(StopTime());
    }

    IEnumerator StopTime()
    {
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 0.001f;
        yield return new WaitForSeconds(0.00025f);
        Time.timeScale = 1f;
    }

    public void Wall(bool wall)
    {
        PlayerAni.SetBool("Wall", wall);
    }
}
