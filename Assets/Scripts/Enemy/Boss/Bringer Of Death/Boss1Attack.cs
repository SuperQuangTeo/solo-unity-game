using UnityEngine;

public class Boss1Attack : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D attackCollider;

    public void activeAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void deactiveAttackCollider()
    {
        attackCollider.enabled = false;
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
