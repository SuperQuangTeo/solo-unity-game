using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAutoSpawner : MonoBehaviour
{
    public List<SpawnObject> spawnObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (SpawnObject obj in spawnObjects) { 
            if (obj.prefab != null && obj.spawnPoint != null)
            {
                GameObject enemy = ObjectPool.Instance.SpawnFromPool(obj.prefab, obj.spawnPoint.position, Quaternion.identity);
                if (enemy != null)
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if(enemyAI != null)
                    {
                        enemyAI.originPrefab = obj.prefab;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
    public Transform spawnPoint;
}
