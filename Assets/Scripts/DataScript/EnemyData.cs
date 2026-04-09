using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public float maxHealth = 10f;
    public float moveSpeed = 2f;
    public bool isBoss = false;
    public bool canPatrol = true;

}
