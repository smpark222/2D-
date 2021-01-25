using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownWallForEnemy : MonoBehaviour
{
    //public Transform checkPos;
    BoxCollider2D downWallBox;

    bool Time_Set = false;
    float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        downWallBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Time_Set)
        //{
        //    time = 0f;
        //    if (checkPos.position.y > this.transform.position.y+0.21f)
        //        downWallBox.enabled = true;
        //    else
        //        downWallBox.enabled = false;
        //}
        //else
        //{
        //    time += Time.deltaTime;
        //}

        //if (time > 0.3f)
        //{
        //    Time_Set = false;
        //}

        //if(Input.GetKey(KeyCode.S) &&Input.GetKeyDown(KeyCode.Space))
        //{
        //    downWallBox.enabled = false;
        //    Time_Set = true;
        //}
    }
}
