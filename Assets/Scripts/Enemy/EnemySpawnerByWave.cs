using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerByWave : MonoBehaviour, ISaveable
{
    public static EnemySpawnerByWave Instance { get; private set; }

    public List<Wave> waves;
    public int currentWaveIndex = 0;
    public float timeBetweenWaves = 3f;
    public int remainingEnemies = 0;
    private bool isWaveInProgress = false;
    private bool isFinishWave = false;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private BlockingWay blockingWay;

    [Header("Trigger Key Settings")]
    [SerializeField] private GameObject triggerKeyPrefab;
    [SerializeField] private Transform keySpawnPoint;
    private GameObject currentKey;

    public bool IsRoomChallengeActive => isWaveInProgress;

    public static event Action OnRoomChallengeActive; 

    void Awake()
    {
        Instance = this;
        blockingWay = gameObject.GetComponent<BlockingWay>();
    }
    void Start()
    {
        if (isFinishWave)
        {
            if (blockingWay != null) blockingWay.SetStateWay(false);
        }
        else
        {
            ResetTriggerKey();
        }
    }

    public void StartWave()
    {
        if (!isWaveInProgress)
        {
            OnRoomChallengeActive?.Invoke();
            AudioManager.Instance.PlayMusic("EnemyWaveRoom");
            AudioManager.Instance.Stop("InGame");
            StartCoroutine(SpawnWaveCoroutine());
            blockingWay.SetStateWay(true);
        }
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        isWaveInProgress = true;

        while (currentWaveIndex < waves.Count)
        {
            SpawnEnemiesInWave(waves[currentWaveIndex]);
            yield return new WaitUntil(() => remainingEnemies <= 0); // Wait until all enemies in the current wave are defeated

            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                isWaveInProgress = false;
                isFinishWave = true;
                blockingWay.SetStateWay(false);
                GameManager.Instance.SaveGame();
                AudioManager.Instance.PlayMusic("InGame");
                AudioManager.Instance.Stop("EnemyWaveRoom");
                yield break; // Exit the coroutine if all waves are completed
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemiesInWave(Wave wave)
    {
        foreach (EnemySpawnInfo enemySpawnInfo in wave.enemiesToSpawn)
        {
            StartCoroutine(WaitAfterSummonEffect(enemySpawnInfo));
            remainingEnemies++;
        }
    }

    private IEnumerator WaitAfterSummonEffect(EnemySpawnInfo enemySpawnInfo)
    {
        var summonEffect = EffectManager.Instance.GetEffect<SummonEffect>();
        if (summonEffect != null)
        {
            summonEffect.StartEffect(enemySpawnInfo.spawnPoint);
        }

        yield return new WaitForSeconds(1.5f);

        GameObject enemy = ObjectPool.Instance.SpawnFromPool(enemySpawnInfo.prefab, enemySpawnInfo.spawnPoint.position, Quaternion.identity);
        if (enemy != null)
        {
            activeEnemies.Add(enemy);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.originPrefab = enemySpawnInfo.prefab;
                enemyAI.OnEnemyDie = OnEnemyDie; // Subscribe to the enemy die event
            }
        }
    }

    public void ResetSpawner()
    {
        if (!isWaveInProgress) return;
        currentKey = null;
        StopAllCoroutines();
        ResetTriggerKey();

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                ObjectPool.Instance.ReturnToPool(enemy);
            }
        }
        activeEnemies.Clear();

        currentWaveIndex = 0;
        remainingEnemies = 0;
        isWaveInProgress = false;

        AudioManager.Instance.PlayMusic("InGame");
        AudioManager.Instance.Stop("EnemyWaveRoom");

        if (blockingWay != null) blockingWay.SetStateWay(false);
    }

    private void ResetTriggerKey()
    {
        if (currentKey == null)
        {
            currentKey = ObjectPool.Instance.SpawnFromPool(triggerKeyPrefab, keySpawnPoint.position, Quaternion.identity);
        }
    }

    private void OnEnemyDie()
    {
        remainingEnemies--;
    }

    public void SaveData(ref GameData data)
    {
        data.isFinishWave = this.isFinishWave;
    }

    public void LoadData(GameData data)
    {
        this.isFinishWave = data.isFinishWave;
        if (Time.timeSinceLevelLoad > 0.2f && ObjectPool.Instance != null)
        {
            if (isFinishWave)
            {
                if (currentKey != null)
                {
                    ObjectPool.Instance.ReturnToPool(currentKey);
                    currentKey = null;
                }
            }
            else
            {
                ResetTriggerKey();
            }
        }
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