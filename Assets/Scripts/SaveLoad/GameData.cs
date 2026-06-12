using UnityEngine;

[System.Serializable]
public class GameData
{
    public float[] playerPosition;
    public int currentHeart;
    public int coin;
    public int key;
    public int heart;
    public int element;
    public bool isFinishWave;
    public bool isOpenChest;

    public GameData()
    {
        playerPosition = new float[3] { 0f, 0f, 0f };
        currentHeart = 5;
        coin = 0;
        key = 0;
        heart = 0;
        element = 0;
        isFinishWave = false;
        isOpenChest = false;
    }
}
