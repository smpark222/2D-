using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    PunchMan,
    KnifeMan,
    GunMan,
    RifleMan,
    ShieldMan
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy",order = int.MaxValue)]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private EnemyType enemyType;
    public EnemyType EnemyType { get { return enemyType; } }


    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }


    [SerializeField]
    private GameObject attackPrefab;
    public GameObject AttackPrefab { get { return attackPrefab; } }

    [SerializeField]
    private float attackCoolTime;
    public float AttackCoolTime { get { return attackCoolTime; } }

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }

    [SerializeField]
    private RuntimeAnimatorController animator;
    public RuntimeAnimatorController Animator { get { return animator; } }
}
