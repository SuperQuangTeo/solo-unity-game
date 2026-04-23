using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    [Header("Boss Stats")]
    public float maxHealth = 100f;
    public float speed = 5f;
    public int damage = 1;
}
