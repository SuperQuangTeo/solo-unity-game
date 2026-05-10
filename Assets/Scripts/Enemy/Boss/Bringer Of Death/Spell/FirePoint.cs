using UnityEngine;

public class FirePoint : MonoBehaviour
{
    [SerializeField] private int damge = 1;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakenDamage(damge);
        }
    }
}
