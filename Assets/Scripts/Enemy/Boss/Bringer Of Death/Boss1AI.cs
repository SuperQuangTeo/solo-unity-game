using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;

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
    public Transform playerTransform;
    public GameObject spellPrefab;

    private BossState bossState = BossState.Chase;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackTime = 2f;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private float castTime = 15f;
    [SerializeField] private float castTimer = 0f;

    [SerializeField] private bool isAttack = false;
    [SerializeField] private bool isCasting = false;

    private float attackAnimLength;
    private float castAnimLength;

    private Animator animator;

    private BossRoomBound bossRoomBound;

    private void Awake()
    {
        bossRoomBound = GetComponent<BossRoomBound>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        attackAnimLength = GetAnimationLength(animator, "Attack");
        castAnimLength = GetAnimationLength(animator, "Cast");
        Debug.Log("minX: " + BossRoomBound.instance.minX);
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

    private void HandleChase()
    {
        if (Mathf.Abs(bossRb.linearVelocity.x) > 0.1f)
        {
            animator.SetBool("running", true);
        }

        float direaction = Mathf.Sign(playerRb.position.x - bossRb.position.x);
        transform.localScale = new Vector3(-direaction, 1, 1);
        bossRb.linearVelocity = new Vector2(BossData.speed * direaction, bossRb.linearVelocity.y);
    }

    private void HandleDecisionLogic()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerRb.position);

        bool canCast = castTimer >= castTime;
        bool canAttack = attackTimer >= attackTime;

        // Xử lý xoay mặt về phía player liên tục khi đang Chase hoặc chuẩn bị ra đòn
        FlipTowardsPlayer();

        // NẾU PLAYER Ở GẦN (Trong tầm đánh thường)
        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("canCast" + canCast);
            Debug.Log("canAttack" + canAttack);
            // Linh hoạt: Nếu cả 2 chiêu đều sẵn sàng, có 30% tỷ lệ dùng Spell ở cự ly gần để gây bất ngờ
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
                // Nếu đang chờ hồi chiêu, đứng im (hoặc lùi lại tuỳ bạn)
                bossRb.linearVelocity = new Vector2(0, bossRb.linearVelocity.y);
                animator.SetBool("running", false);
            }
        }
        // NẾU PLAYER Ở XA (Ngoài tầm đánh thường)
        else
        {
            if (canCast)
            {
                // Người chơi ở xa và Spell đã sẵn sàng -> Dùng Spell
                SwitchState(BossState.Cast);
            }
            else
            {
                // Không có chiêu gì xài được -> Chạy lại gần
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
        GameObject spell = ObjectPool.Instance.SpawnFromPool(spellPrefab, new Vector3(playerTransform.position.x, BossRoomBound.instance.maxY - 3), Quaternion.identity);
        spell.GetComponent<SpellAnim>()?.CallSpell();
    }

    private IEnumerator CastCoroutine()
    {
        isCasting = true;
        animator.SetBool("running", false); // Tắt anim chạy

        animator.SetTrigger("cast");

        // Đợi cho đến khi animation cast kết thúc (thường dài hơn)
        float waitTime = castAnimLength > 0 ? castAnimLength : 1.5f; // Tránh lỗi nếu không tìm thấy clip
        yield return new WaitForSeconds(waitTime);

        // Reset sau khi cast
        castTimer = 0;
        isCasting = false;

        // Trở lại trạng thái Chase để quyết định tiếp
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
        // Tránh lỗi chớp giật mặt khi khoảng cách quá nhỏ xíu
        if (Mathf.Abs(playerRb.position.x - bossRb.position.x) > 0.1f)
        {
            float direction = Mathf.Sign(playerRb.position.x - bossRb.position.x);
            transform.localScale = new Vector3(-direction, 1, 1);
        }
    }

    public float GetAnimationLength(Animator animator, string name)
    {
        foreach(var clip in animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == name)
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
            bossState = BossState.Chase;
            Debug.Log(bossState);
        }
        else
        {
            bossState = BossState.Idle;
            Debug.Log(bossState);
        }
    }
}
