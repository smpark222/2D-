using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy
{
    public EnemyType EnemyType { get; set; }

    public float AttackRange { get; set; }

    public GameObject AttackPrefab { get; set; }

    public float AttackCoolTime { get; set; }
    
    public Sprite sprite { get; set; }
    
    public RuntimeAnimatorController animator { get; set; }
}
