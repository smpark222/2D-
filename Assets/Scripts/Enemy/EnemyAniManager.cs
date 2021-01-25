using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAniManager : MonoBehaviour
{
    Animator enemyAni;

    // Start is called before the first frame update
    void Start()
    {
        enemyAni = this.GetComponent<Animator>();
    }
    public bool GetAim()
    {
        return enemyAni.GetBool("Aim");
    }
    public void Aim(bool aim)
    {
        enemyAni.SetBool("Aim", aim);
    }
    public void Attack()
    {
        enemyAni.SetTrigger("Attack");
    }
    public void Run(bool run)
    {
        enemyAni.SetBool("DetectPlayer", run);
    }
    public void Walk(bool walk)
    {
        enemyAni.SetBool("Walk", walk);
    }

    public void Turn()
    {
        enemyAni.SetTrigger("Turn");
    }
    public void Death()
    {
        enemyAni.SetTrigger("Death");
        enemyAni.SetBool("Dead", true);
    }
    public void Revive()
    {
        enemyAni.SetBool("Dead", false);
    }
    public void Ground()
    {
        enemyAni.SetTrigger("Ground");
    }
}
