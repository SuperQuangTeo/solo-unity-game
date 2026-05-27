using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public List<ShopItemData> shopItems;

    public GameObject shopPanel;
    public GameObject slotContainer;
    public GameObject slotPrefab;

    public Button closeButton;

    void Start()
    {
        if(shopItems == null || shopItems.Count == 0)
        {
            return;
        }
        GenerateItems();
    }

    private void GenerateItems()
    {
        foreach(var item in shopItems)
        {
            GameObject itemPrefab = Instantiate(slotPrefab, slotContainer.transform);
            if (itemPrefab != null)
            {
                itemPrefab.GetComponent<ShopSlotUI>()?.Initialize(item);
            }
            else
            {
                continue;
            }
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
    }
    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

}
