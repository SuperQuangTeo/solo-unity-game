using System;
using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float boxWidth = 0.1f; // Chiều rộng kiểm tra
    public float boxHeight = 0.6f; // Chiều cao hộp kiểm tra
    public float checkDistance = 0.05f;

    public bool isRolling = false;
    public bool canRoll = true;
    public float rollTime = 0.1f;
    public PhysicsMaterial2D slideMaterial;
    public PhysicsMaterial2D stopMaterial;

    [SerializeField] private float rollCooldown = 0.5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rollDistance = 10f;
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
        float scaleX = Mathf.Abs(transform.localScale.x); // Lấy giá trị dương
        float scaleY = transform.localScale.y;
        //Jump
        if (Input.GetKeyDown(KeyCode.K) && IsGround())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            _animator.SetTrigger("jump");
            attackScript.EndCombo();    
        }
        //Roll
        if (Input.GetKeyDown(KeyCode.L) && !isRolling && canRoll)
        {
            Roll();
        }   
        //Move
        if (!attackScript.isAttacking && !isRolling)
        {
            rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
        }
        else if(attackScript.isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (IsGround() && moveX == 0 && rb.linearVelocity.y <= 0.01f)
        {
            characterCollider.sharedMaterial = stopMaterial;
        }
        else
        {
            characterCollider.sharedMaterial = slideMaterial;
        }

        // Lật hướng nhân vật nếu đi ngược
        if (moveX != 0)
        {
            _animator.SetBool("isRunning", true);
            transform.localScale = new Vector3(Mathf.Sign(moveX) * scaleX, scaleY, 1);
            
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
    }
    
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector2 boxSize = new Vector2(boxWidth, boxHeight);
            Gizmos.DrawWireCube(groundCheck.position + Vector3.down * checkDistance / 2f, boxSize);
        }
    }

    public bool IsGround() { 
        Vector2 origin = groundCheck.position;
        Vector2 boxSize = new Vector2(boxWidth, boxHeight);
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, checkDistance, groundLayer);
        return hit.collider != null;
    }

    public void Roll()
    {
        if (attackScript.isAttacking || isRolling) return;
        StartCoroutine(RollCoroutime());
    }

    private IEnumerator RollCoroutime()
    {
        canRoll = false;
        isRolling = true;
        float rollDirection = transform.localScale.x;
     
        rb.linearVelocity = new Vector2(rollDirection * rollDistance, 2);
        _animator.SetTrigger("roll");
        yield return new WaitForSeconds(rollTime);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isRolling = false;
        yield return new WaitForSeconds(rollCooldown);

        canRoll = true;
    }

}
