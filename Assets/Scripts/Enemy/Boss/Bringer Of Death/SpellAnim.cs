using System.Collections;
using UnityEngine;

public class SpellAnim : MonoBehaviour
{
    private Animator _spellPrepare;
    private Animator _spellAttack;
    public GameObject spellPrepare;
    public GameObject spellAttack;

    [SerializeField] private float spellTime = 3f;
    [SerializeField] private float spellTimer = 0f;
    [SerializeField] private bool isFinishAnim = false;
    private float spellAttackAnimLength = 0f;

    private void Awake()
    {
        _spellPrepare = spellPrepare.GetComponent<Animator>();
        _spellAttack = spellAttack.GetComponent<Animator>();
        spellAttackAnimLength = GetAnimationLength(_spellAttack, "Run");
    }

    void Update()
    {
        if (!isFinishAnim)
        {
            spellTimer += Time.deltaTime;
            if (spellTimer >= spellTime)
            {
                isFinishAnim = true;
            }

        }
        else if (isFinishAnim)
        {
            isFinishAnim = false;
            ObjectPool.Instance.ReturnToPool(gameObject);
            spellAttack.SetActive(true);
        }
    }
    public void CallSpell()
    {
        spellTimer = 0;
        _spellPrepare.Play("Run");
        _spellAttack.Play("Run");
        StartCoroutine(DisableAttackSpellAnim());
    }

    private IEnumerator DisableAttackSpellAnim()
    {
        yield return new WaitForSeconds(spellAttackAnimLength);
        spellAttack.SetActive(false);
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
