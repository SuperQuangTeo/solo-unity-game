using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }
    public GameObject pointA;
    public GameObject pointB;
    public Animator _animator;
    public LayerMask playerLayer;
    public Transform playerTransform;
    public PhysicsMaterial2D slideMaterial;
    public PhysicsMaterial2D stopMaterial;

    [SerializeField] private float speed = 4f;
    [SerializeField] private Rigidbody2D enemyRb;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private Transform currentPoint;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackTimer = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private bool isAttacking = false;
    private EnemyState currentState = EnemyState.Patrol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = attackCooldown;
        currentPoint = pointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        Debug.Log("velocity " + enemyRb.linearVelocity);

        //if(enemyRb.linearVelocity.x == 0 &&  enemyRb.linearVelocity.y <= 0.01f)
        //{
        //    enemyCollider.sharedMaterial = stopMaterial;
        //}
        //else
        //{
        //    enemyCollider.sharedMaterial = slideMaterial;
        //}

        if (attackTimer <= 2 && !isAttacking)
        {
            attackTimer -= Time.deltaTime;
        }
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                if (distanceToPlayer <= chaseRange)
                {
                    currentState = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                if (IsInChasingRange())
                {
                    HandleChase();
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
                if (distanceToPlayer > attackRange + 0.2f)
                {
                    currentState = EnemyState.Chase;

                }
                break;
        }

    }

    void HandlePatrol()
    {
        if (currentPoint == pointB.transform)
        {
            enemyRb.linearVelocity = new Vector2(speed, enemyRb.linearVelocity.y);
        }
        else
        {
            enemyRb.linearVelocity = new Vector2(-speed, enemyRb.linearVelocity.y);
        }
        if (Vector2.Distance(transform.position, currentPoint.position) <= 1f && currentPoint == pointB.transform)
        {
            currentPoint = pointA.transform;
            Flip();
        }
        if (Vector2.Distance(transform.position, currentPoint.position) <= 1f && currentPoint == pointA.transform)
        {
            currentPoint = pointB.transform;
            Flip();
        }
    }

    void HandleChase()
    {

        FlipTowardsPlayer();
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

    }

    void HandleAttack()
    {
        //attackTimer -= Time.deltaTime;
        if (!isAttacking && attackTimer <= 0)
        {
            _animator.SetTrigger("attack");
            isAttacking = true;
        }
        if (isAttacking)
        {
            attackTimer = attackCooldown;
            isAttacking = false;
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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void FlipTowardsPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        if (direction.x < 0) currentPoint = pointA.transform;
        if (direction.x > 0) currentPoint = pointB.transform;
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


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
