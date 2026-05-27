using UnityEngine;

public interface ISaveable
{
    void SaveData(ref GameData data);

    void LoadData(GameData data);
}
