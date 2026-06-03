using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static string saveFilePath = Application.persistentDataPath + "saveData.json";

    public static void SaveGame(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(json);

            //Debug.Log("Load Game Done");
            return loadedData;
        }
        else
        {
            //Debug.LogWarning("Not found, create new file");
            return new GameData();
        }

    }

    public static bool IsFileExist()
    {
        return File.Exists(saveFilePath);
    }
    public static void DeleteFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
