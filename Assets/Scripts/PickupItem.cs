using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType
    {
        Heart,
        Coin,
    }

    public ItemType itemType;
    public int value = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            switch (itemType)
            {
                case ItemType.Heart:
                    PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
                    playerHealth.Heal(value);
                    break;
                case ItemType.Coin:
                    break;
            }
            Destroy(gameObject);
        }
    }
}
