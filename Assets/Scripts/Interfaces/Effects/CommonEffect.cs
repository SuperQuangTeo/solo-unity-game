using UnityEngine;

public class CommonEffect : MonoBehaviour,IEffect
{
    public StatusEffectData effectData;

    public Animator _animator;

    private Transform targetAnchor;
    private float timeRemaining;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (targetAnchor != null && targetAnchor.gameObject.activeInHierarchy)
        {
            transform.position = targetAnchor.position;
        }
        else
        {
            StopEffect();
        }

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            StopEffect();
        }
    }

    public void StopEffect()
    {
        targetAnchor = null;
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    public void StartEffect(Transform target)
    {
        targetAnchor = target;

        if (targetAnchor != null)
        {
            transform.position = targetAnchor.position;
        }

        gameObject.SetActive(true);
        if (_animator != null) _animator.Play("Run", -1, 0f);

        timeRemaining = effectData.duration;
    }
}
