using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    Rigidbody2D pR;
    // Start is called before the first frame update
    void Start()
    {
        pR = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(pR.velocity != Vector2.zero)
        {
            Invoke("ChangeStatic", 15f);
        }
        if(GameObject.Find("Player").GetComponent<TimeBody>().isRewinding)
        {
            ChangeStatic();
        }
        
    }
    
    void ChangeStatic()
    {
        pR.bodyType = RigidbodyType2D.Static;
    }
}
