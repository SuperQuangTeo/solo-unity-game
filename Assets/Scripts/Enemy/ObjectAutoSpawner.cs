using System.Collections.Generic;
using UnityEngine;

public class ObjectAutoSpawner : MonoBehaviour
{
    public enum ObjectType { Enemy, Item }
    public ObjectType m_ObjectType;
    public List<SpawnObject> spawnObjects;

    private List<GameObject> activeEnemies = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObjects();
        GameManager.Instance.ResetAllSaveableObjects();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (m_ObjectType == ObjectType.Enemy)
        {
            PlayerHealth.OnPlayerDeath += ResetAndRespawn;
        }
    }

    private void OnDisable()
    {
        if (m_ObjectType == ObjectType.Enemy)
        {
            PlayerHealth.OnPlayerDeath -= ResetAndRespawn;
        }
    }

    private void ResetAndRespawn()
    {
        ClearActiveEnemies();
        SpawnObjects();
    }

    private void ClearActiveEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                ObjectPool.Instance.ReturnToPool(enemy);
            }
        }

        activeEnemies.Clear();
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
                            activeEnemies.Add(enemy);
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
