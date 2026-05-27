using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameData gameData;
    private List<ISaveable> saveableObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        this.saveableObjects = new List<ISaveable>();
        this.gameData = new GameData();
    }

    private void Start()
    {
        this.saveableObjects = FindAllSaveableObjects();
        foreach (ISaveable obj in saveableObjects)
        {
            obj.SaveData(ref gameData);
        }
        //StartCoroutine(LoadGameWithDelay()); 
    }

    public void SaveGame()
    {
        //if (gameData == null) gameData = new GameData();
        foreach (ISaveable obj in saveableObjects)
        {
            obj.SaveData(ref gameData);
        }

        SaveLoadManager.SaveGame(gameData);
    }

    public void LoadGame()
    {
        this.gameData = SaveLoadManager.LoadGame();
        if (gameData == null)
        {
            gameData = new GameData();
        }
        if (gameData.playerPosition[0] == 0 && gameData.playerPosition[1] == 0 && gameData.playerPosition[2] == 0)
        {
            GameObject startPoint = GameObject.Find("StartingPoint");
            if (startPoint != null)
            {
                Vector3 spawnPos = startPoint.transform.position;
                UpdateCheckPointPos(spawnPos);
            }
        }
        foreach (ISaveable obj in saveableObjects)
        {
            obj.LoadData(gameData);
        }
    }

    private List<ISaveable> FindAllSaveableObjects()
    {
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveable>();
        return new List<ISaveable>(saveables);
    }

    public void UpdateCheckPointPos(Vector3 pos)
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        gameData.playerPosition[0] = pos.x;
        gameData.playerPosition[1] = pos.y;
        gameData.playerPosition[2] = pos.z;
    }

    public void UpdateDataInMemoryOnly()
    {
        if (gameData == null) gameData = new GameData();
        foreach (ISaveable obj in saveableObjects)
        {
            obj.SaveData(ref gameData);
        }
    }
    public void RespawnPlayer()
    {
        foreach (ISaveable obj in saveableObjects)
        {
            obj.LoadData(gameData);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            this.saveableObjects = FindAllSaveableObjects();
            LoadGame();
        }
        else
        {
            this.saveableObjects = new List<ISaveable>();
        }
    }
}
