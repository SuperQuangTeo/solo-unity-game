using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerByWave : MonoBehaviour
{
    public static EnemySpawnerByWave Instance { get; private set; }

    public List<Wave> waves;
    public int currentWaveIndex = 0;
    public float timeBetweenWaves = 3f;
    private float waveTimer = 0f;
    public int remainingEnemies = 0;
    private bool isWaveInProgress = false;

    private BlockingWay blockingWay;

    void Awake()
    {
        Instance = this;
        blockingWay = gameObject.GetComponent<BlockingWay>();
    }
    public void StartWave()
    {
        if (!isWaveInProgress) { 
            StartCoroutine(SpawnWaveCoroutine());
            blockingWay.SetStateWay(true);
        }
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        isWaveInProgress = true;

        while(currentWaveIndex < waves.Count)
        {
            SpawnEnemiesInWave(waves[currentWaveIndex]);
            yield return new WaitUntil(() => remainingEnemies <= 0); // Wait until all enemies in the current wave are defeated

            currentWaveIndex++;

            if(currentWaveIndex >= waves.Count)
            {
                Debug.Log("All waves completed!");
                isWaveInProgress = false;
                blockingWay.SetStateWay(false);
                yield break; // Exit the coroutine if all waves are completed
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemiesInWave(Wave wave)
    {
        foreach(EnemySpawnInfo enemySpawnInfo in wave.enemiesToSpawn)
        {
            GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemySpawnInfo.prefab, enemySpawnInfo.spawnPoint.position, Quaternion.identity);
            if(enemy != null)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if(enemyAI != null)
                {
                    enemyAI.originPrefab = enemySpawnInfo.prefab;
                    enemyAI.OnEnemyDie = OnEnemyDie; // Subscribe to the enemy die event
                }
            }
            remainingEnemies++;
        }
    }

    private void OnEnemyDie()
    {
        remainingEnemies--;
    }
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject prefab;
    public Transform spawnPoint;
}

[System.Serializable]
public class Wave
{
    public List<EnemySpawnInfo> enemiesToSpawn;
}