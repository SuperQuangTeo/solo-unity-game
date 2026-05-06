using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType
    {
        Heart,
        Coin,
        Key,
        TriggerWaveKey,
    }

    public ItemType itemType;
    public int value = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            PlayerInventory inventory = collision.collider.GetComponent<PlayerInventory>();
            switch (itemType)
            {
                case ItemType.Heart:
                    if (playerHealth.currentHeart < playerHealth.numberOfHeart)
                    {
                        playerHealth.Heal(value);
                    }
                    else if (playerHealth.currentHeart == playerHealth.numberOfHeart)
                    {
                        inventory.AddHeart(value);
                    }
                    break;
                case ItemType.Coin:
                    inventory.AddCoin(value);
                    break;
                case ItemType.Key:
                    inventory.AddKey(value);
                    break;
                case ItemType.TriggerWaveKey:
                    inventory.AddKey(value);
                    EnemySpawnerByWave.Instance.StartWave();
                    break;

            }
            Destroy(gameObject);
        }
    }
}
