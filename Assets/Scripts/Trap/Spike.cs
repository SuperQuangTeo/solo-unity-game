using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakenDamage(damage);
        }

    }
}
