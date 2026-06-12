using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour, IInteractable, ISaveable
{
    public List<ItemInChest> itemsInChest;
    public Sprite openedChestSprite;
    public Sprite closeChestSprite;
    [SerializeField] private bool isRequiredKey = false;
    private SpriteRenderer spriteRenderer;

    private bool isOpen = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        var playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        if (isOpen)
        {
            return;
        }
        if (isRequiredKey && playerInventory.key <= 0)
        {
            return;
        }
        if (isRequiredKey)
        {
            playerInventory.SpendKey(1);
        }
        if (itemsInChest != null)
        {
            foreach(var item in itemsInChest)
            {
                if (item != null)
                {
                    int amount = Random.Range(item.minAmount, item.maxAmount + 1);
                    for(int i = 0; i < amount; i++)
                    {
                        Vector2 randomDir = Random.insideUnitCircle.normalized;

                        float distance = Random.Range(0.5f, 1.5f);

                        Vector3 spawnPos = transform.position + (Vector3)randomDir * distance + Vector3.up * 2f;
                        ObjectPool.Instance.SpawnFromPool(item.itemPrefab, spawnPos, Quaternion.identity);
                    }
                }
            }
        }
        isOpen = true;
        if (isOpen)
        {
            spriteRenderer.sprite = openedChestSprite;
        }
    }

    public void LoadData(GameData data)
    {
        if (data.isOpenChest)
        {
            spriteRenderer.sprite = openedChestSprite;
        }
        else
        {
            spriteRenderer.sprite = closeChestSprite;
        }
        this.isOpen = data.isOpenChest;
    }

    public void NextLine()
    {
        throw new System.NotImplementedException();
    }

    public void SaveData(ref GameData data)
    {
        data.isOpenChest = this.isOpen;
    }
}
