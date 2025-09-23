using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Scriptable Objects/AttackData")]
public class AttackData : ScriptableObject
{
    public String attackName;
    public float damageAmount;
    public DAMAGE_TYPE type;
}

public enum DAMAGE_TYPE
{
    Melee,
    Magic,
    Projectile
}
