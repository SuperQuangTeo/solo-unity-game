using UnityEngine;

public class HealingEffect : MonoBehaviour,IEffect
{

    public float duration = 1f;

    public Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void StartEffect(Transform target)
    {
        transform.position = target.position;
        gameObject.SetActive(true);
        _animator.Play("Run", -1, 0f);
        
        CancelInvoke(nameof(StopEffect));
        Invoke(nameof(StopEffect), duration);
    }

    public void StopEffect()
    {
        gameObject.SetActive(false);

    }

}
