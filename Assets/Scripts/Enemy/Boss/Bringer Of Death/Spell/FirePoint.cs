using UnityEngine;

public class FirePoint : MonoBehaviour
{
    [SerializeField] private int damge = 1;
    [SerializeField] private float appearanceTime = 15f;
    [SerializeField] private float appearanceTimer = 0f;

    void Update()
    {
        appearanceTimer += Time.deltaTime;
        if(appearanceTimer >= appearanceTime)
        {
            ObjectPool.Instance.ReturnToPool(gameObject);
            appearanceTimer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakenDamage(damge);
        }
    }

    private void OnEnable()
    {
        Boss1Health.OnBossDeath += DisableObjAfterBossDeath;
    }

    private void OnDisable()
    {
        Boss1Health.OnBossDeath -= DisableObjAfterBossDeath;
    }

    private void DisableObjAfterBossDeath()
    {
        ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
