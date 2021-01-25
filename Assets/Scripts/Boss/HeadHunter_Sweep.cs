using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunter_Sweep : MonoBehaviour
{
    BossAniManager bossAni;
    Rigidbody2D bossRigid;
    BoxCollider2D bossBox;

    HeadHunter headHunter;
    public Coroutine sweepRoutine;
    public GameObject laser;

    // Start is called before the first frame update
    void Start()
    {
        headHunter = GetComponent<HeadHunter>();
        bossAni = GetComponent<BossAniManager>();
        bossRigid = GetComponent<Rigidbody2D>();
        bossBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Sweep()
    {
        if(!headHunter.isDead)
            sweepRoutine = StartCoroutine(SweepStart());
    }


    IEnumerator SweepStart()
    {
        yield return new WaitForSeconds(1f);
        Recover();
        yield return new WaitForSeconds(1f);
        transform.position = new Vector2(61f, 8.75f);
        transform.localScale = new Vector2(-1f, transform.localScale.y);

        bossRigid.gravityScale = 0f;
        TeleportOutSweep();

        yield return new WaitForSeconds(0.8f);

        GameObject laser1 = Instantiate(laser, new Vector2(transform.position.x, transform.position.y + 0.2f), Quaternion.identity);
        StartCoroutine(LaserCircle(laser1));

        yield return new WaitForSeconds(0.8f);
        TeleportInSweep();

        yield return new WaitForSeconds(0.35f);
        bossAni.Sweep(false);
        yield return new WaitForSeconds(0.65f);

        transform.localScale = new Vector2(1f, transform.localScale.y);
        transform.position = new Vector2(80f, 8.75f);
        TeleportOutSweep();

        yield return new WaitForSeconds(0.9f);

        GameObject laser2 = Instantiate(laser, new Vector2(transform.position.x, transform.position.y + 0.2f), Quaternion.identity);
        StartCoroutine(LaserCircle(laser2));

        yield return new WaitForSeconds(1.1f);
        bossAni.Sweep(false);

        yield return new WaitForSeconds(0.4f);
        bossRigid.gravityScale = 3f;
        headHunter.isAttacking = false;

        while (!headHunter.isGrounded)
        {
            yield return new WaitForFixedUpdate();
        }
        headHunter.isHurt = false;

        //GameObject laser = Instantiate(lasershot, go.transform.position, Quaternion.identity);
    }

    public void Recover()
    {
        
        bossAni.Recover();

    }
    public void TeleportInSweep()
    {
        bossAni.ReSweep();
        bossBox.enabled = false;
    }
    public void TeleportOutSweep()
    {
        bossAni.Sweep(true);
        bossBox.enabled = true;
    }

    IEnumerator LaserCircle(GameObject go)
    {
        go.transform.localScale = transform.localScale;

        if (transform.localScale.x < 0f)
        {
            go.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 40f));

            while (go.transform.rotation.eulerAngles.z < 180f)
            {
                go.transform.Rotate(Vector3.forward, Time.deltaTime * 160f);
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            go.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 320f));

            while (go.transform.rotation.eulerAngles.z > 180f)
            {
                go.transform.Rotate(-Vector3.forward, Time.deltaTime * 160f);
                yield return new WaitForFixedUpdate();
            }
        }
        yield return new WaitForFixedUpdate();

        go.GetComponent<Laser>().destroy = true;
    }
}
