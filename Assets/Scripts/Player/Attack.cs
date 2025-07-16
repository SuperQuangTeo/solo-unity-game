using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator _animator;
    public bool isAttacking = false;
    public float checkDistance = 0.05f;
    private int comboStep = 0;
    [SerializeField] private float comboTimer = 0f;
    [SerializeField] private float comboWindow = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        comboTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.J) && IsGround() && !isAttacking)
        {
            if (comboStep == 0)
            {
                comboStep = 1;
                StartAttack();
            }
            else if (comboStep == 1 && comboTimer > 0)
            {
                comboStep = 2;
                StartAttack();
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        _animator.SetInteger("comboStep", comboStep);
        _animator.SetTrigger("attack");
        comboTimer = comboWindow;
    }

    private bool IsGround()
    {
        return _animator.GetBool("isGrounded");
    }

    public void AllowNextCombo()
    {
        isAttacking = false ;
    }

    public void EndCombo()
    {
        comboStep = 0;
        isAttacking = false;
        comboTimer = 0;
        _animator.SetInteger("comboStep", 0);
    }
}
