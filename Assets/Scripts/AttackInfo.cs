using System;
using UnityEngine;

[Serializable]
public class AttackInfo
{
    public String name;
    public String description;
    
    // boundaries;
    [SerializeField] private Vector2[] attackBoundaries;
    [SerializeField] private Vector2[] attackBoundaryOffsets;
    [SerializeField] private Vector2[] attackKnockBacks;

    public Vector2[] AttackBoundaries => attackBoundaries;
    public Vector2[] AttackBoundaryOffsets => attackBoundaryOffsets;
    public Vector2[] AttackKnockBacks => attackKnockBacks;
    
    // attack's properties like damage, etc.
    [SerializeField] private float[] attackDamages;
    [SerializeField] private float[] attackStunTimes;
    [SerializeField] private float[] attackDashes;

    public float[] AttackDamages => attackDamages;
    public float[] AttackStunTimes => attackStunTimes;
    public float[] AttackDashes => attackDashes;
    
    // slow motion info;
    [SerializeField] private SlowMotionInfo[] slowMotionInfos;

    public SlowMotionInfo[] SlowMotionInfos => slowMotionInfos;
}