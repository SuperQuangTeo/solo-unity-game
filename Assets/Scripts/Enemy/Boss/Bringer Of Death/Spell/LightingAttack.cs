using UnityEngine;

public class LightingAttack : MonoBehaviour
{
    [SerializeField] private BoxCollider2D lightingCollider;


    public void ActiveLightingCollider()
    {
        if (lightingCollider != null)
        {
            lightingCollider.enabled = true;
        }
    }

    public void DeactiveLightingCollider()
    {
        if (lightingCollider != null)
        {
            lightingCollider.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakenDamage(1);
            }
        }
    }
}
