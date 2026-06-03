using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator _animator;
    public bool isAttacking = false;
    public bool isLockActionWhenAttack = false;
    public float checkDistance = 0.05f;
    public GameObject attackPoint;
    public LayerMask enemyLayer;
    private int comboStep = 0;
    [SerializeField] private float comboTimer = 0f;
    [SerializeField] private float comboWindow = 1f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 2f;

    [SerializeField] private float burnEffectTotalTime = 3f;
    [SerializeField] private float burnEffectPerTime = 1f;
    [SerializeField] private float burnEffectDamage = 1f;

    [SerializeField] private float shockEffectTime = 5f;
    [SerializeField] private float shockEffectPercent = 30f;

    [SerializeField] private float effectCooldown = 8f;

    private PlayerElemental playerElemental;

    void Awake()
    {
        playerElemental = GetComponent<PlayerElemental>();
    }

    void Update()
    {
        comboTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.J) && IsGround() && !isAttacking)
        {
            if (comboStep == 0)
            {
                comboStep = 1;
                StartAttack();
                AudioManager.Instance.PlaySFX("PlayerAttack1");
            }
            else if (comboStep == 1 && comboTimer > 0)
            {
                comboStep = 2;
                StartAttack();
                AudioManager.Instance.PlaySFX("PlayerAttack2");
            }
        }
    }

    void StartAttack()
    {
        isLockActionWhenAttack = true;
        isAttacking = true;
        _animator.SetInteger("comboStep", comboStep);
        _animator.SetTrigger("attack");
        comboTimer = comboWindow;
    }

    private bool IsGround()
    {
        return _animator.GetBool("isGrounded");
    }

    public void DoAttack()
    {
        float finalDamage = playerElemental.getDamageBonus(attackDamage);
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.transform.position, attackRange, enemyLayer);
        if (hit != null)
        {
            hit.gameObject.GetComponent<EnemyAI>()?.TakeDamage(finalDamage);
            hit.gameObject.GetComponent<Boss1Health>()?.TakeDamge(finalDamage);

            ApplyElementEffect(hit.gameObject);
        }

    }

    private void ApplyElementEffect(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            if (enemyAI.CanReceiveElementEffect())
            {
                enemyAI.StartEffectedelementTimer(effectCooldown);
            }
            else return;
        }
        else
        {
            Boss1Health bossHealth = enemy.GetComponent<Boss1Health>();
            if (bossHealth != null)
            {
                if (bossHealth.CanReceiveElementEffect())
                {
                    bossHealth.StartEffectedelementTimer(effectCooldown);
                }
                else
                {
                    return;
                }
            }
        }
        Transform vfxAnchor = enemy.transform.Find("VFXAnchor");
        Transform targetTransform = vfxAnchor != null ? vfxAnchor : enemy.transform;
        switch (playerElemental.currentElemental)
        {
            case PlayerElemental.ElementalType.Fire:
                var burnEffect = EffectManager.Instance.GetEffect<BurnEffect>();
                if(burnEffect != null)
                {
                    burnEffect.StartEffect(targetTransform);
                }
                StartCoroutine(BurnEffect(enemy));
                break;
            case PlayerElemental.ElementalType.Electric:
                var shockEffect = EffectManager.Instance.GetEffect<ShockEffect>();
                if (shockEffect != null)
                {
                    shockEffect.StartEffect(targetTransform);
                }
                StartCoroutine(ShockEffect(enemy));
                break;
        }
    }

    public void AllowNextCombo()
    {
        isAttacking = false;
    }

    public void EndCombo()
    {
        comboStep = 0;
        isAttacking = false;
        comboTimer = 0;
        _animator.SetInteger("comboStep", 0);
        isLockActionWhenAttack = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }

    private IEnumerator BurnEffect(GameObject enemy)
    {
        for (int i = 1; i <= burnEffectTotalTime; i++)
        {
            yield return new WaitForSeconds(burnEffectPerTime);
            enemy.GetComponent<EnemyAI>()?.TakeDamage(burnEffectDamage);
            enemy.GetComponent<Boss1Health>()?.TakeDamge(burnEffectDamage);
        }
    }
    private IEnumerator ShockEffect(GameObject enemy)
    {
        enemy.GetComponent<EnemyAI>()?.ReduceAttackSpeedAndMoveSpeedByPercent(shockEffectPercent);
        enemy.GetComponent<Boss1AI>()?.ReduceAttackSpeedAndMoveSpeedByPercent(shockEffectPercent);
        yield return new WaitForSeconds(shockEffectTime);
        enemy.GetComponent<EnemyAI>()?.ResetSpeedAndMoveSpeed();
        enemy.GetComponent<Boss1AI>()?.ResetSpeedAndMoveSpeed();
    }
}
