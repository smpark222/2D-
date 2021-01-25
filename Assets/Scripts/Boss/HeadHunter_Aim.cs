using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHunter_Aim : MonoBehaviour
{
    BossAniManager bossAniManager;

    Animator HeadHunterAni;
    Transform playerPos;
    Vector2 dir;

    public GameObject laserLine;
    public GameObject lasershot;

    bool aiming = false;

    HeadHunter headHunter;

    // Start is called before the first frame update
    void Start()
    {
        bossAniManager = GetComponent<BossAniManager>();
        HeadHunterAni = GetComponent<Animator>();
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        headHunter = GetComponent<HeadHunter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HeadHunterAni.GetBool("Aim"))
        {
            if (aiming)
            {
                dir = (playerPos.position - this.transform.position).normalized;
                bossAniManager.Aiming(dir.x, dir.y);
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LaserAimShot();
        }
    }

    public void LaserAimShot()
    {
        StartCoroutine(Laser());
    }

    public void LaserRotate(GameObject go)
    {
        if (go != null)
        {
            float angle;
            Vector2 dirNo = dir.normalized;
            if (dir.x > 0)
            {
                angle = Mathf.Atan2(dirNo.y, dirNo.x) * Mathf.Rad2Deg;
            }
            else
            {
                angle = Mathf.Atan2(-dirNo.y, -dirNo.x) * Mathf.Rad2Deg;
            }
            go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    IEnumerator LaserAimRot(GameObject go)
    {
        while (aiming)
        {
            LaserRotate(go);
            yield return null;
        }

    }
    IEnumerator Laser()
    {
        bossAniManager.Aim(true);
        aiming = true;
        yield return new WaitForSeconds(0.7f);
        
        GameObject go = Instantiate(laserLine, new Vector2(transform.position.x, transform.position.y + 0.2f), Quaternion.identity);
        
        StartCoroutine(LaserAimRot(go));
       
        go.transform.localScale = this.transform.localScale;

        Transform[] laserLines = go.GetComponentsInChildren<Transform>();
        List<Transform> laserList = new List<Transform>();
        List<Coroutine> routineList = new List<Coroutine>();

        for (int i = 0; i < laserLines.Length; i++)
        {
            laserList.Insert(i, laserLines[i]);
        }

        while (laserList.Count != 0)
        {
            for (int i = 0; i < 5; i++)
            {
                if (laserList.Count > 0)
                {
                    int randomNum = Random.Range(0, laserList.Count);
                    StartCoroutine(LaserLine(laserList[randomNum], 1.3f));
                    laserList.RemoveAt(randomNum);
                }
                else
                    break;
            }
            yield return new WaitForSeconds(0.01f);

        }
        yield return new WaitForSeconds(0.5f);

        laserList = new List<Transform>();

        for (int i = 0; i < laserLines.Length; i++)
        {
            laserList.Insert(i, laserLines[i]);
        }
        aiming = false;

        while (laserList.Count != 0)
        {
            for (int i = 0; i < 5; i++)
            {
                if (laserList.Count > 0)
                {

                    int randomNum = Random.Range(0, laserList.Count);
                    routineList.Add(StartCoroutine(LaserLine(laserList[randomNum], 0f)));
                    laserList.RemoveAt(randomNum);
                }
                else
                    break;
            }
            yield return new WaitForSeconds(0.01f);

        }
        yield return new WaitForSeconds(0.3f);

        GameObject laser = Instantiate(lasershot, go.transform.position, Quaternion.identity);
        laser.transform.rotation = go.transform.rotation;
        laser.transform.localScale = new Vector2(go.transform.localScale.x, 1f);


        while (routineList.Count != 0)
        {
            StopCoroutine(routineList[0]);
            routineList.RemoveAt(0);
        }
        Destroy(go.gameObject);

        yield return new WaitForSeconds(0.4f);
        bossAniManager.Aim(false);
        yield return new WaitForSeconds(0.1f);
        laser.GetComponent<Laser>().destroy = true;
        if (!headHunter.isHurt)
        {
            headHunter.isAttacking = false;
        }


    }

    IEnumerator LaserLine(Transform tLaser, float target)
    {
        while (Mathf.Abs(tLaser.localScale.y - target) > 0.01f)
        {
            tLaser.localScale = new Vector2(tLaser.localScale.x, Mathf.Lerp(tLaser.localScale.y, target, Time.deltaTime * 13f));
            yield return new WaitForSeconds(0.001f);
        }
        tLaser.localScale = new Vector2(tLaser.localScale.x, target);
    }
}
