using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunArm : MonoBehaviour
{
    Transform playerPos;

    Vector2 dir;
    Vector2 dirNo;
    float angle;


    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        dir = playerPos.position - this.transform.position;
        dirNo = dir.normalized;

        angle = Mathf.Clamp(angle, -70f, 70f);
        if (dir.x > 0)
        {
            angle = Mathf.Atan2(dirNo.y, dirNo.x) * Mathf.Rad2Deg;
        }
        else
        {
            angle = Mathf.Atan2(-dirNo.y, -dirNo.x) * Mathf.Rad2Deg;
        }
        

        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
