using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Boss1AI : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Chase,
        Attack,
        Cast,
        Death,
    }
    public BossData BossData;
    public Rigidbody2D bossRb;
    public Rigidbody2D playerRb;
    public PlayerHealth playerHealth;
    public Transform playerTransform;
    public Transform bossTransform;
    private Vector3 bossPos;
    public GameObject spellPrefab;

    private BossState bossState = BossState.Chase;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float baseAttackTime = 2f;
    [SerializeField] private float attackTime = 2f;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private float tempAttackTime = 0f;
    [SerializeField] private float baseCastTime = 10f;
    [SerializeField] private float castTime = 10f;
    [SerializeField] private float castTimer = 0f;
    [SerializeField] private float tempCastTime = 0f;

    [SerializeField] private bool isAttack = false;
    [SerializeField] private bool isCasting = false;

    private float attackAnimLength;
    private float castAnimLength;

    private Animator animator;

    private BossRoomBound bossRoomBound;

    private bool? IsHealthBossBelowAHalf => GetComponent<Boss1Health>()?.IsHealthBossBelowAHalf;

    private void Awake()
    {
        bossRoomBound = GetComponent<BossRoomBound>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        attackAnimLength = GetAnimationLength(animator, "Attack");
        castAnimLength = GetAnimationLength(animator, "Cast");
        tempCastTime = castTime;
        tempAttackTime = attackTime;

        baseSpeed = BossData.speed;
        baseAttackTime = attackTime;
        baseCastTime = castTime;
        bossPos = bossTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerRb.position);

        if (attackTimer >= 0f && !isAttack)
        {
            attackTimer += Time.deltaTime;
        }
        if (castTimer >= 0f)
        {
            castTimer += Time.deltaTime;
        }

        switch (bossState)
        {
            case BossState.Idle:
                bossRb.linearVelocity = new Vector2(0, bossRb.linearVelocity.y);
                animator.SetBool("running", false);
                break;
            case BossState.Chase:
                HandleDecisionLogic();
                break;
            case BossState.Attack:
                HandleAttack();
                break;
            case BossState.Cast:
                HandleCast();
                break;
            case BossState.Death:
                break;
        }
    }

    //private void HandleChase()
    //{
    //    if (Mathf.Abs(bossRb.linearVelocity.x) > 0.1f)
    //    {
    //        animator.SetBool("running", true);
    //    }

    //    float direaction = Mathf.Sign(playerRb.position.x - bossRb.position.x);
    //    transform.localScale = new Vector3(-direaction, 1, 1);
    //    bossRb.linearVelocity = new Vector2(BossData.speed * direaction, bossRb.linearVelocity.y);
    //}

    private void HandleDecisionLogic()
    {
        if (playerHealth.IsDeath)
        {
            SwitchState(BossState.Idle);
            return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, playerRb.position);

        bool canCast = castTimer >= castTime;
        bool canAttack = attackTimer >= attackTime;

        FlipTowardsPlayer();

        if (distanceToPlayer <= attackRange)
        {
            if (canCast && canAttack && Random.value < 0.5f)
            {
                SwitchState(BossState.Cast);
            }
            else if (canAttack)
            {
                SwitchState(BossState.Attack);
            }
            else
            {
                bossRb.linearVelocity = new Vector2(0, bossRb.linearVelocity.y);
                animator.SetBool("running", false);
            }
        }
        else
        {
            if (canCast)
            {
                SwitchState(BossState.Cast);
            }
            else
            {
                HandleChaseMovement();
            }
        }
    }

    private void HandleChaseMovement()
    {
        animator.SetBool("running", true);
        float direction = Mathf.Sign(playerRb.position.x - bossRb.position.x);
        bossRb.linearVelocity = new Vector2(BossData.speed * direction, bossRb.linearVelocity.y);
    }

    private void HandleAttack()
    {
        if (attackTimer >= attackTime && !isAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private void HandleCast()
    {
        if (castTimer > castTime && !isCasting)
        {
            StartCoroutine(CastCoroutine());
        }
    }

    public void SummonSpell()
    {
        Vector3 spawnPos = new Vector3(playerTransform.position.x, BossRoomBound.instance.maxY - 3, 0);
        GameObject spell = ObjectPool.Instance.SpawnFromPool(spellPrefab, spawnPos, Quaternion.identity);

        float distanceToGround = spawnPos.y - BossRoomBound.instance.minY;

        SpellAnim spellAnim = spell.GetComponent<SpellAnim>();

        if (spellAnim != null)
        {
            SpriteRenderer lightingSprite = spellAnim.spellAttack.GetComponent<SpriteRenderer>();
            float defaultHeight = lightingSprite.sprite.bounds.size.y;

            float scaleY = distanceToGround / defaultHeight;

            spellAnim.spellAttack.transform.localScale = new Vector3(spell.transform.localScale.x, scaleY, spell.transform.localScale.z);
        }

        spell.GetComponent<SpellAnim>()?.CallSpell(IsHealthBossBelowAHalf, new Vector3(playerTransform.position.x, BossRoomBound.instance.minY, 0));
    }

    public void ReduceAttackSpeedAndMoveSpeedByPercent(float percent)
    {
        attackTime = tempAttackTime * (1 + percent / 100);
        castTime = tempCastTime * (1 + percent / 100);
        BossData.speed = baseSpeed * (1 - percent / 100);
    }

    public void IncreaseAttackSpeedAndMoveSpeedByPercent(float percent)
    {
        attackTime = baseAttackTime * (1 - percent / 100);
        BossData.speed = baseSpeed * (1 + percent / 100);
        if (IsHealthBossBelowAHalf == true)
        {
            castTime = tempCastTime * (1 - percent / 100);
        }
        else castTime = baseCastTime * (1 - percent / 100);
        castTimer = castTime;
    }
    public void ResetSpeedAndMoveSpeed()
    {
        attackTime = baseAttackTime;
        tempAttackTime = attackTime;
        tempCastTime = castTime;
        castTime = baseCastTime;
        BossData.speed = baseSpeed;
    }

    public void ResetPos()
    {
        ResetSpeedAndMoveSpeed();
        animator.Rebind();
        animator.Update(0f);
        bossTransform.position = bossPos;
    }

    private void BossPhaseTwo()
    {
        castTime = tempCastTime / 2f;
        attackTime = tempAttackTime / 1.5f;
        tempAttackTime = baseAttackTime /1.5f;
        tempCastTime = baseCastTime /2f;
    }

    private void OnEnable() => Boss1Health.OnBossHealthBelowAHalf += BossPhaseTwo;
    private void OnDisable() => Boss1Health.OnBossHealthBelowAHalf -= BossPhaseTwo;

    private IEnumerator CastCoroutine()
    {
        isCasting = true;
        animator.SetBool("running", false);

        animator.SetTrigger("cast");

        float waitTime = castAnimLength > 0 ? castAnimLength : 1.5f;
        yield return new WaitForSeconds(waitTime);

        castTimer = 0;
        isCasting = false;

        SwitchState(BossState.Chase);
    }

    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("attack");
        isAttack = true;

        yield return new WaitForSeconds(attackAnimLength);

        attackTimer = 0;
        isAttack = false;

        SwitchState(BossState.Chase);
    }

    private void SwitchState(BossState newState)
    {
        bossState = newState;
        if (newState == BossState.Attack || newState == BossState.Cast)
        {
            bossRb.linearVelocity = new Vector2(0, bossRb.linearVelocity.y);
        }
    }

    private void FlipTowardsPlayer()
    {
        if (Mathf.Abs(playerRb.position.x - bossRb.position.x) > 0.1f)
        {
            float direction = Mathf.Sign(playerRb.position.x - bossRb.position.x);
            transform.localScale = new Vector3(-direction, 1, 1);
        }
    }

    public float GetAnimationLength(Animator animator, string name)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    public void IsPlayerInBound(bool isInBound)
    {
        if (isInBound)
        {
            SwitchState(BossState.Chase);
        }
        else
        {
            SwitchState(BossState.Idle);
        }
    }
}
