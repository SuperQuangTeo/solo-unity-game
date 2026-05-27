using System;
using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour,ISaveable
{
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    public LayerMask wallLayer;
    public float boxWidthJump = 0.1f;
    public float boxHeightJump = 0.6f;
    public float boxWidthWallSlide = 0.6f;
    public float boxHeightWallSlide = 0.1f;
    public float checkDistance = 0.05f;

    public bool isRolling = false;
    public bool canRoll = true;
    public float rollTime = 0.1f;
    public PhysicsMaterial2D slideMaterial;
    public PhysicsMaterial2D stopMaterial;

    [SerializeField] private float rollCooldown = 0.5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float slideWallJumpForce = 7f;
    [SerializeField] private float rollDistance = 10f;
    [SerializeField] private float slideWallVelocity = -3f;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private Attack attackScript;
    [SerializeField] private Collider2D characterCollider;

    // Update is called once per frame
    void Update()
    {
        // Di chuyển trái/phải
        float moveX = Input.GetAxisRaw("Horizontal");
        _animator.SetFloat("yVelocity", rb.linearVelocity.y);
        _animator.SetBool("isGrounded", IsGround());
        bool isSliding = IsWallSlide() && !IsGround() && rb.linearVelocity.y < 0;
        bool isTouchingWallRight = rb.linearVelocity.x > 0;
        _animator.SetBool("isSlidingWall", isSliding);

        if (isSliding && !isRolling)
        {
            SlideWall();
        }
        //Jump
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (IsGround())
            {
                AudioManager.Instance.PlaySFX("PlayerJump");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                _animator.SetTrigger("jump");
                attackScript.EndCombo();
            }
            else if (IsWallSlide() && rb.linearVelocity.y < 0) {
                AudioManager.Instance.PlaySFX("PlayerJump");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, slideWallJumpForce);
                _animator.SetTrigger("jump");
                attackScript.EndCombo();
            }
        }
        //Roll
        if (Input.GetKeyDown(KeyCode.L) && !isRolling && canRoll)
        {
            attackScript.EndCombo();
            Roll(moveX);
        }
        //Move
        if (!attackScript.isLockActionWhenAttack && !isRolling)
        {
            rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
            // Lật hướng nhân vật nếu đi ngược
            if (moveX != 0)
            {
                _animator.SetBool("isRunning", true);
                if (moveX > 0 && !isFacingRight && !attackScript.isLockActionWhenAttack)
                {
                    Flip();
                }
                else if (moveX < 0 && isFacingRight && !attackScript.isLockActionWhenAttack)
                {
                    Flip();
                }
            }
            else
            {
                _animator.SetBool("isRunning", false);
            }
        }
        else if (attackScript.isLockActionWhenAttack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            _animator.SetBool("isRunning", false);
        }

        if (IsGround() && moveX == 0 && rb.linearVelocity.y <= 0.01f)
        {
            characterCollider.sharedMaterial = stopMaterial;
        }
        else
        {
            characterCollider.sharedMaterial = slideMaterial;
        }

    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector2 boxSize = new Vector2(boxWidthJump, boxHeightJump);
            Gizmos.DrawWireCube(groundCheck.position + Vector3.down * checkDistance / 2f, boxSize);
        }
        if (wallCheckRight != null)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = new Vector2(boxWidthWallSlide, boxHeightWallSlide);

            // Tính vị trí trung tâm của BoxCast khi va vào tường
            Vector3 center = wallCheckRight.position + (Vector3)(Vector2.right * checkDistance / 2f);

            Gizmos.DrawWireCube(center, boxSize);
        }
        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = new Vector2(boxWidthWallSlide, boxHeightWallSlide);

            // Tính vị trí trung tâm của BoxCast khi va vào tường
            Vector3 center = wallCheckLeft.position + (Vector3)(Vector2.left * checkDistance / 2f);

            Gizmos.DrawWireCube(center, boxSize);
        }
    }

    public bool IsGround()
    {
        Vector2 origin = groundCheck.position;
        Vector2 boxSize = new Vector2(boxWidthJump, boxHeightJump);
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, checkDistance, groundLayer);
        return hit.collider != null;
    }

    public bool IsWallSlide()
    {
        Vector2 boxSize = new Vector2(boxWidthWallSlide, boxHeightWallSlide);
        Vector2 originRight = wallCheckRight.position;
        Vector2 originLeft = wallCheckLeft.position;

        RaycastHit2D hitRight = Physics2D.BoxCast(originRight, boxSize, 0f, Vector2.right, checkDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.BoxCast(originLeft, boxSize, 0f, Vector2.left, checkDistance, wallLayer);
        return hitRight.collider != null || hitLeft.collider != null;
    }

    public void Roll(float direction)
    {
        if (attackScript.isAttacking || isRolling) return;
        AudioManager.Instance.PlaySFX("PlayerRoll");
        StartCoroutine(RollCoroutime(direction));
    }

    private IEnumerator RollCoroutime(float rollDirection)
    {
        canRoll = false;
        isRolling = true;
        if (rollDirection == 0)
        {
            rollDirection = transform.localScale.x;
        }
        rb.linearVelocity = new Vector2(rollDirection * rollDistance, 2);
        _animator.SetTrigger("roll");
        yield return new WaitForSeconds(rollTime);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isRolling = false;
        yield return new WaitForSeconds(rollCooldown);

        canRoll = true;
    }

    public void SlideWall()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, slideWallVelocity);
    }

    public void PlayPlayerRunSound()
    {
        // Chỉ phát nếu đang ở trên mặt đất
        if (IsGround())
        {
            AudioManager.Instance.PlaySFX("PlayerRun");
        }
    }

    public void PlayPlayerSlideWallSound()
    {
        if (IsWallSlide())
        {
            AudioManager.Instance.PlaySFX("PlayerSlideWall");
        }
    }

    public void SaveData(ref GameData data)
    {

    }

    public void LoadData(GameData data)
    {
        Vector3 pos = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        this.transform.position = pos;
    }
}
