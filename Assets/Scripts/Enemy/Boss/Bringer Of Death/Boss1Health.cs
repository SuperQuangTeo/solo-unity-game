using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Boss1Health : MonoBehaviour
{
    [SerializeField] private float currentHealth = 0;

    public BossData bossData;

    public Image healthFill;
    public Image easeHealthBar;
    public float lerpSpeed = 5f;

    private bool isDeath = false;

    private Animator animator;

    public bool? IsHealthBossBelowAHalf => currentHealth < bossData.maxHealth / 2;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        resetBossHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (easeHealthBar.fillAmount != healthFill.fillAmount)
        {
            easeHealthBar.fillAmount = Mathf.Lerp(easeHealthBar.fillAmount, healthFill.fillAmount, lerpSpeed * Time.deltaTime);
        }
        if(currentHealth <= 0 && !isDeath)
        {
            Death();
        }
    }

    public void TakeDamge(float damge)
    {
        currentHealth -= damge;
        healthFill.fillAmount -= (damge/ bossData.maxHealth);
        GetComponent<SimpleFlash>()?.Flash();
    }

    public void resetBossHealth()
    {
        currentHealth = bossData.maxHealth;
        healthFill.fillAmount = currentHealth;
    } 

    void Death()
    {
        if (isDeath) return;
        isDeath = true;
        animator.SetTrigger("death");
        float deathAnimation = GetAnimationLength(animator, "Death");
        StartCoroutine(DeathCoroutine(deathAnimation));
    }

    private IEnumerator DeathCoroutine(float second)
    {
        yield return new WaitForSeconds(second);
        gameObject.SetActive(false);
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
