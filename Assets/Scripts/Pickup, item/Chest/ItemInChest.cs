using UnityEngine;

[CreateAssetMenu(fileName = "ItemInChest", menuName = "Scriptable Objects/ItemInChest")]
public class ItemInChest : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;
    public int minAmount;
    public int maxAmount;
}
