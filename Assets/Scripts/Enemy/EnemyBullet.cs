using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 1;
    private Vector2 direction;
    private Rigidbody2D rb;
    [HideInInspector]
    public GameObject originPrefab;

    [SerializeField] private float lifeTime = 3f;
    private float lifeTimer;

    private Animator _animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            ObjectPool.Instance.ReturnToPool(gameObject);
        }
    }

    public void Fire(Vector2 dir)
    {
        direction = dir.normalized;
        gameObject.SetActive(true);
        rb.linearVelocity = speed * direction;
        //xoay dan huong vao player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _animator.Play("Fire");

        lifeTimer = lifeTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ObjectPool.Instance.ReturnToPool(gameObject);
            collision.GetComponent<PlayerHealth>()?.TakenDamage(damage);
        }
    }
}
