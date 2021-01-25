using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    public bool playerDead;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        playerDead = GameObject.Find("Player").GetComponent<TimeBody>().isRewinding;

        if (playerDead)
        {
            Destroy(gameObject, 1f);
        }
    }
}
