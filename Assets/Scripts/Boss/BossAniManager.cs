using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAniManager : MonoBehaviour
{
    [SerializeField]
    HeadHunter_Aim _Aim;
    [SerializeField]
    HeadHunter_JumpAttack _JumpAttack;
    [SerializeField]
    HeadHunter_Sweep _Sweep;

    Animator bossAni;
    // Start is called before the first frame update
    void Start()
    {
        bossAni = GetComponent<Animator>();
    }

    public void WallJumpReset()
    {
        bossAni.ResetTrigger("WallJump");
    }
    public void JumpReset()
    {
        bossAni.ResetTrigger("Jump");
    }

    public void Aim(bool aim)
    {
        bossAni.SetBool("Aim", aim);
    }

    public void Aiming(float x, float y)
    {
        bossAni.SetFloat("X", x);
        bossAni.SetFloat("Y", y);
    }
    public void Jump()
    {
        bossAni.SetTrigger("Jump");
    }

    public void Wall(bool isWall)
    {
        bossAni.SetBool("Wall", isWall);
    }

    public void WallJump()
    {
        bossAni.SetTrigger("WallJump");
    }

    public void Ground(bool isGround)
    {
        bossAni.SetBool("Ground", isGround);
    }
    public void Sweep(bool isSweep)
    {
        bossAni.SetBool("Sweep", isSweep);
    }
    public void ReSweep()
    {
        bossAni.SetTrigger("ReSweep");
    }
    public void Recover()
    {
        bossAni.SetTrigger("Recover");
    }
    public void RecoverReset()
    {
        bossAni.ResetTrigger("Recover");
    }

    public void JumpAttack()
    {
        _JumpAttack.JumpAttack();
    }
    public void AimAttack()
    {
        _Aim.LaserAimShot();
    }

    public void Sweep()
    {
        _Sweep.Sweep();
    }
    public void Death(bool death)
    {
        bossAni.SetBool("Death", death);
    }
    public void Hurt()
    {
        bossAni.SetTrigger("Hurt");
    }
}
