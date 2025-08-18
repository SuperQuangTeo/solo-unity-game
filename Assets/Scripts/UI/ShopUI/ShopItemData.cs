using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Scriptable Objects/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemImage;
    public int price;
    public int quantity;
}
