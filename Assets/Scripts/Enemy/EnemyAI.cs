using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        MeleeShield,
    }
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Block,
        Death
    }
    //public GameObject pointA;
    //public GameObject pointB;
    public Animator _animator;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Transform playerTransform;
    public Transform groundCheck;
    public Transform wallCheck;
    public PhysicsMaterial2D slideMaterial;
    public PhysicsMaterial2D stopMaterial;
    public Action OnEnemyDie;

    public GameObject bulletPrefab;
    [HideInInspector]
    public GameObject originPrefab;

    public List<LootItem> lootTable = new List<LootItem>();

    [SerializeField] private float baseSpeed = 4f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Rigidbody2D enemyRb;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private Transform currentPoint;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float baseAttackCooldown = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackTimer = 2f;
    [SerializeField] private float blockTime = 2f;
    [SerializeField] private float blockTimer = 0f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float totalHealth = 10f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool isDeath = false;
    private EnemyState currentState = EnemyState.Patrol;
    public EnemyType enemyType = EnemyType.Melee;
    private float lastAttackedTime = -999f;
    private bool isMovingRight = true;
    private float effectedCooldownTimer = 0f;


    public void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseSpeed = speed;
        baseAttackCooldown = attackCooldown;
        attackTimer = attackCooldown;
        //currentPoint = pointB.transform;
        currentHealth = totalHealth;
    }

    private void OnEnable()
    {
        ResetEnemyAfterRespawn();
    }

    private void ResetEnemyAfterRespawn()
    {
        currentHealth = totalHealth;
        isDeath = false;
        isAttacking = false;
        currentState = EnemyState.Patrol;
        if (enemyCollider != null) enemyCollider.enabled = true;

        if (_animator != null)
        {
            _animator.ResetTrigger("death");
            _animator.Rebind();
            _animator.Update(0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(effectedCooldownTimer > 0f)
        {
            effectedCooldownTimer -= Time.deltaTime;
        }
        bool isOnGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        //if(enemyRb.linearVelocity.x == 0 &&  enemyRb.linearVelocity.y <= 0.01f)
        //{
        //    enemyCollider.sharedMaterial = stopMaterial;
        //}
        //else
        //{
        //    enemyCollider.sharedMaterial = slideMaterial;
        //}
        Vector2 direction = isMovingRight ? Vector2.right : Vector2.left;

        if (!isBlocking)
        {
            blockTimer -= Time.deltaTime;
        }
        if (currentHealth <= 0 && !isDeath)
        {
            currentState = EnemyState.Death;
        }

        if (attackTimer <= attackCooldown && !isAttacking)
        {
            attackTimer -= Time.deltaTime;
        }
        switch (currentState)
        {

            case EnemyState.Patrol:
                HandlePatrol(direction);
                RunAnim();
                if (distanceToPlayer <= chaseRange && (enemyType == EnemyType.Melee || enemyType == EnemyType.MeleeShield))
                {
                    currentState = EnemyState.Chase;
                }
                else if (distanceToPlayer <= attackRange + 0.2f && enemyType == EnemyType.Ranged)
                {
                    currentState = EnemyState.Attack;
                }
                break;
            case EnemyState.Chase:
                //bool isOnGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

                if (IsInChasingRange() && !isAttacking && isOnGround)
                {
                    HandleChase(direction);
                    RunAnim();
                }
                else if (IsInChasingRange() && !isOnGround)
                {
                    currentState = EnemyState.Patrol;
                }

                if (distanceToPlayer > chaseRange + 0.2f || !IsInChasingRange())
                {
                    currentState = EnemyState.Patrol;
                }
                if (distanceToPlayer <= attackRange + 0.2f)
                {
                    currentState = EnemyState.Attack;
                }
                break;
            case EnemyState.Attack:
                HandleAttack();
                RunAnim();
                if (distanceToPlayer > attackRange + 0.2f && (enemyType == EnemyType.Melee || enemyType == EnemyType.MeleeShield))
                {
                    currentState = EnemyState.Chase;
                }
                else if (distanceToPlayer > attackRange + 0.2f && enemyType == EnemyType.Ranged)
                {
                    currentState = EnemyState.Patrol;
                }
                break;
            case EnemyState.Block:
                HandleBlock();
                break;
            case EnemyState.Death:
                Death();
                StartCoroutine(DropItemAfterDie());
                break;
        }

    }

    void HandlePatrol(Vector2 direction)
    {
        //Vector2 direction = isMovingRight ? Vector2.right : Vector2.left;
        enemyRb.linearVelocity = new Vector2(direction.x * speed, enemyRb.linearVelocity.y);

        bool isOnGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool isHittingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, wallLayer);

        if (!isOnGround || isHittingWall)
        {
            Flip();
        }

    }

    void HandleChase(Vector2 direction)
    {
        FlipTowardsPlayer();
        //transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        //float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
        enemyRb.linearVelocity = new Vector2(direction.x * speed, enemyRb.linearVelocity.y);
    }

    void HandleAttack()
    {
        FlipTowardsPlayer();

        if (!isAttacking && attackTimer <= 0)
        {
            _animator.SetTrigger("attack");
            isAttacking = true;
        }
        //else if (isAttacking)
        //{
        //    attackTimer = attackCooldown;
        //    isAttacking = false;
        //}
    }

    void HandleBlock()
    {

        if (Time.time - lastAttackedTime < blockTime)
        {
            if (!isBlocking)
            {
                float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
                enemyRb.linearVelocity = new Vector2(direction, enemyRb.linearVelocity.y);
                _animator.SetBool("isBlocking", true);
                isBlocking = true;
                blockTimer = blockTime;
            }
            enemyRb.linearVelocity = Vector2.zero;
        }
        else
        {
            if (isBlocking)
            {
                _animator.SetBool("isBlocking", false);
                isBlocking = false;

            }
            currentState = EnemyState.Patrol;
        }

    }

    public void ShootBullet()
    {
        GameObject bullet = ObjectPool.Instance.SpawnFromPool(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (playerTransform.position - firePoint.position).normalized;
        bullet.GetComponent<EnemyBullet>().originPrefab = bulletPrefab;
        bullet.GetComponent<EnemyBullet>()?.Fire(dir);
    }

    void RunAnim()
    {
        if (Mathf.Abs(enemyRb.linearVelocity.x) > 0.1f)
        {
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
    }

    public void DoDamage()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (playerHit != null)
        {
            playerHit.GetComponent<PlayerHealth>()?.TakenDamage(attackDamage);
        }
    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void FlipTowardsPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        if (direction.x < 0) isMovingRight = false;
        if (direction.x > 0) isMovingRight = true;
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
    }

    private bool IsInChasingRange()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer);
        if (playerInRange != null)
        {
            Vector2 directionToPlayer = playerTransform.position - transform.position;
            float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);
            LayerMask ObstacleLayer = LayerMask.GetMask("Ground");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, ObstacleLayer);

            if (hit.collider == null)
            {
                return true;
            }
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        if (enemyType == EnemyType.MeleeShield)
        {
            bool isEnemyFacingLeft = transform.localScale.x < 0;
            bool isPlayerOnLeft = playerTransform.position.x < transform.position.x;
            bool isFacingPlayer = (isEnemyFacingLeft && isPlayerOnLeft) || (!isEnemyFacingLeft && !isPlayerOnLeft);
            //Debug.LogWarning("isfacingPlayer " + (Mathf.Sign(playerTransform.position.x - transform.position.x) < 0));

            if (isFacingPlayer && blockTimer >= 0)
            {
                lastAttackedTime = Time.time;
                currentState = EnemyState.Block;
                return;
            }
        }

        currentHealth -= damage;
        if (currentHealth > 0 && !isDeath)
        {
            blockTimer = blockTime;
            _animator.SetTrigger("hit");
            isAttacking = false;
            attackTimer = 0;
            //Debug.Log("Health: " + currentHealth);
        }
    }
    public void EndAttack()
    {
        isAttacking = false;
        attackTimer = attackCooldown;
    }

    public void Death()
    {
        if (isDeath) return;
        isDeath = true;
        _animator.SetTrigger("death");
        enemyRb.linearVelocity = Vector2.zero;
    }

    void InstantiateItem(GameObject lootItem)
    {
        if (lootItem)
        {
            GameObject itemClone = ObjectPool.Instance.SpawnFromPool(lootItem, transform.position, Quaternion.identity);
        }
    }

    public void ReduceAttackSpeedAndMoveSpeedByPercent(float percent)
    {
        attackCooldown = baseAttackCooldown * (1 - percent / 100);
        attackTimer = attackCooldown;
        speed = baseSpeed * (1 - percent / 100);
    }

    public void IncreaseAttackSpeedAndMoveSpeedByPercent(float percent)
    {
        attackCooldown = baseAttackCooldown * (1 + percent / 100);
        attackTimer = attackCooldown;
        speed = baseSpeed * (1 + percent / 100);
    }
    public void ResetSpeedAndMoveSpeed()
    {
        attackCooldown = baseAttackCooldown;
        attackTimer = baseAttackCooldown;
        speed = baseSpeed;
    }

    public bool CanReceiveElementEffect()
    {
        return effectedCooldownTimer <= 0;
    }

    public void StartEffectedelementTimer(float effectedElementTime)
    {
        effectedCooldownTimer = effectedElementTime;
    }

    private IEnumerator DropItemAfterDie()
    {
        yield return new WaitForSeconds(1f);
        foreach (LootItem lootItem in lootTable)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= lootItem.dropRate)
            {
                InstantiateItem(lootItem.itemPrefab);
                break;
            }
        }
        OnEnemyDie?.Invoke();
        ObjectPool.Instance.ReturnToPool(gameObject);

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        //Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector2 dir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(dir * wallCheckDistance));
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector2 dir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}
