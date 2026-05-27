using UnityEngine;

public class UIImageAnimator : MonoBehaviour
{
    [SerializeField] private string animationName;
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (animator != null)
        {
            animator.Play(animationName, 0, 0f);
        }
    }
}
