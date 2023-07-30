using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NpcHelper
{
    public string Name { get; }
    public float Hp { get; set; }
    public float Radius { get; }
    public float CollisionDisplacement { get; }
    public float MaxHp { get; }
    public float WeakPoint { get; }
    public float MaxWeakPoint { get; }
    public bool CanBeKockedOut { get; }
    public bool Dizzy { get; }
    public void GetHurt(DamageData damageData);
    public void Move();
    public void Turn(Vector3 direction);
    public void Look(Transform target);
    public void Die();
}
public class AnimationWeight
{
    public string TriggerName;
    public Action Condiction;
}
public class IdelStateInfo
{
    public readonly float randomValue = UnityEngine.Random.value;
    public Vector3 OriginPosition;
    public AnimationWeight[] Animations;

    public IdelStateInfo(Vector3 oriPos, params AnimationWeight[] animations)
    {
        OriginPosition = oriPos;
        Animations = animations;
    }
}
public class FightStateInfo
{
    public readonly float randomValue = UnityEngine.Random.value;
    public AnimationWeight[] Animations;
    public Transform target;

    public FightStateInfo(Transform t, params AnimationWeight[] animations)
    {
        target = t;
        Animations = animations;
    }
}
public class AttackStateInfo
{
    public readonly float randomValue = UnityEngine.Random.value;
    public AnimationWeight[] Animations;
    public float MinAttackDistance;
    public float MaxAttackDistance;
}
public class ChaseStateInfo
{
    public readonly float randomValue = UnityEngine.Random.value;
    public AnimationWeight[] Animations;
    public float KeepAwayDistance;
    public float ApproachDistance;
    public Vector3 velocity;
}
