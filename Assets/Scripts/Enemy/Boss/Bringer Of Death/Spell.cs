using UnityEngine;

public class Spell : MonoBehaviour
{
    private Animator _animator;
    private float spellAnimLength = 0f;
    private float spellTimer = 0f;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        spellAnimLength = GetAnimationLength(_animator, "Spell");
    }

    void Update()
    {
        spellTimer -= Time.deltaTime;
        if (spellTimer <= 0)
        {
            ObjectPool.Instance.ReturnToPool(gameObject);
        }
    }

    public void CallSpell()
    {
        _animator.SetTrigger("run");
        Debug.Log("run spell" + spellAnimLength);

        spellTimer = spellAnimLength;
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
}
