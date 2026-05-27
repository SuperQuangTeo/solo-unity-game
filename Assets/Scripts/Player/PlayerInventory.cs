using UnityEngine;

public class PlayerInventory : MonoBehaviour,ISaveable
{
    public int coin { get; private set; } = 100;
    public int heart { get; private set; } = 0;
    public int key { get; private set; } = 0;
    public int elemental {  get; private set; }

    public void AddCoin(int coin)
    {
        this.coin += coin;
    }

    public void SpendCoin(int amount)
    {
        if (coin >= amount)
        {
            coin -= amount;
        }
        else
        {
            return;
        }
    }

    public void AddHeart(int number)
    {
        heart += number;
    }

    public void SpendHeart(int number)
    {
        if (heart >= number)
        {
            heart -= number;
        }
        else
        {
            return;
        }
    }

    public void AddKey(int number)
    {
        key += number;
    }

    public void SpendKey(int number)
    {
        if(key >= number)
        {
            key -= number;
        }
        else
        {
            return;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.coin = this.coin;
        data.key = this.key;
        data.heart = this.heart;
    }

    public void LoadData(GameData data)
    {
        this.coin = data.coin;
        this.key = data.key;
        this.heart = data.heart;
    }
}
