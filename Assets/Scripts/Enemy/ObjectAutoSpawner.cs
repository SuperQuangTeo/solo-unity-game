using System.Collections.Generic;
using UnityEngine;

public class ObjectAutoSpawner : MonoBehaviour
{
    public enum ObjectType { Enemy, Item }
    public ObjectType m_ObjectType;
    public List<SpawnObject> spawnObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnObjects()
    {
        switch (m_ObjectType)
        {
            case ObjectType.Enemy:
                foreach (SpawnObject obj in spawnObjects)
                {
                    if (obj.prefab != null && obj.spawnPoint != null)
                    {
                        GameObject enemy = ObjectPool.Instance.SpawnFromPool(obj.prefab, obj.spawnPoint.position, Quaternion.identity);
                        if (enemy != null)
                        {
                            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                            if (enemyAI != null)
                            {
                                enemyAI.originPrefab = obj.prefab;
                            }
                        }
                    }
                }
                break;
            case ObjectType.Item:
                foreach (SpawnObject obj in spawnObjects)
                {
                    if (obj.prefab != null && obj.spawnPoint != null)
                    {
                        GameObject item = ObjectPool.Instance.SpawnFromPool(obj.prefab, obj.spawnPoint.position, Quaternion.identity);
                    }
                }
                break;
        }
    }
}

[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
    public Transform spawnPoint;
}
