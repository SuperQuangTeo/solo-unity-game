using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public ShopItemData shopItemData;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemImage;

    public Image[] allImages;
    public void Initialize(ShopItemData itemData)
    {
        shopItemData = itemData;
        if (itemNameText != null)
        {
            itemNameText.text = shopItemData.itemName;
        }
        if (itemPriceText != null)
        {
            itemPriceText.text = shopItemData.price.ToString();
        }
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = shopItemData.description;
        }
        if (itemImage != null && shopItemData.itemImage != null)
        {
            itemImage.sprite = shopItemData.itemImage;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Debug.Log($"Clicked on item: {shopItemData.itemName}");
        BuyItem();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (var img in allImages)
        {
            img.color = new Color(0.7f, 0.7f, 0.7f, img.color.a);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (var img in allImages)
        {
            img.color = new Color(1f, 1f, 1f, img.color.a);
        }
    }

    public void BuyItem()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (shopItemData.price > player.GetComponent<PlayerInventory>().coin)
        {
            //Debug.Log("Not enough coins to buy this item.");
            return;
        }
        if (shopItemData.itemName.Equals("Heart"))
        {
            player.GetComponent<PlayerInventory>().AddHeart(shopItemData.quantity);
        }
        else if (shopItemData.itemName.Equals("Key"))
        {
            player.GetComponent<PlayerInventory>().AddKey(shopItemData.quantity);
        }
        else if (shopItemData.itemName.Equals("FireOrb"))
        {
            player.GetComponent<PlayerElemental>().UseElemental(1);
        }
        else if (shopItemData.itemName.Equals("WaterOrb"))
        {
            player.GetComponent<PlayerElemental>().UseElemental(2);
        }
        else if (shopItemData.itemName.Equals("ElectricOrb"))
        {
            player.GetComponent<PlayerElemental>().UseElemental(3);
        }

        player.GetComponent<PlayerInventory>().SpendCoin(shopItemData.price);

    }
}
