using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private float hitTime = 0.1f;
    [SerializeField] private bool isDeath = false;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float invincibleTime = 1f;
    [SerializeField] private float healTime = 1f;
    [SerializeField] private float healTimer = 0f;

    private bool isStanding = false;
    [SerializeField] private bool isHealing = false;

    public int numberOfHeart { get; private set; } = 5;
    public int currentHeart { get; private set; }


    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameObject heartPrefab;
    public Transform heartPanel;
    public List<Image> hearts = new List<Image>();

    public movement playerMovement;
    public Attack playerAttack;
    public PlayerInventory playerInventory;
    private void Awake()
    {
        playerMovement = GetComponent<movement>();
        playerAttack = GetComponent<Attack>();
        playerRigid = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Start()
    {
        currentHeart = numberOfHeart;
        CreateHeart();
    }


    void Update()
    {
        isStanding = Math.Abs(playerRigid.linearVelocity.x) < 0.01f && Math.Abs(playerRigid.linearVelocity.y) < 0.01f;
        var healEffect = EffectManager.Instance.GetEffect<HealingEffect>();
        updateHeartUI();

        if (currentHeart <= 0 && !isDeath)
        {
            Death();
        }

        if (isHealing)
        {
            healTimer += Time.deltaTime;
            if (healTimer >= healTime)
            {

                UseHeart(1);
                isHealing = false;

                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.H) && isStanding && currentHeart < numberOfHeart && playerInventory.heart > 0)
        {
            isHealing = true;
            if (isHealing && !isDeath && playerMovement.IsGround() && !playerMovement.isRolling && !playerAttack.isAttacking)
            {
                healEffect.StartEffect(transform);
                Debug.Log("Start Healing Effect");
            }
        }
        else if (Input.GetKeyUp(KeyCode.H))
        {

            isHealing = false;
            healTimer = 0f;
            healEffect.StopEffect();

        }
    }

    void CreateHeart()
    {
        for (int i = 0; i < numberOfHeart; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, heartPanel);
            Image heartImg = heartObj.GetComponent<Image>();

            hearts.Add(heartImg);
        }
    }

    void updateHeartUI()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHeart)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void TakenDamage(int damage)
    {
        if (currentHeart > 0 && !isInvincible && !isDeath)
        {
            currentHeart -= damage;
            _animator.SetTrigger("hit");
            StartCoroutine(InvincibleCoroutine());
            StartCoroutine(KnockbackCoroutine());
        }
    }

    public void Heal(int heartNumber)
    {
        if (currentHeart < numberOfHeart)
        {
            currentHeart += heartNumber;
        }
    }

    public void UseHeart(int heartNumber)
    {
        gameObject.GetComponent<PlayerInventory>().SpendHeart(heartNumber);
        Heal(heartNumber);
    }

    public void Death()
    {
        if (isDeath) return;
        isDeath = true;
        _animator.SetTrigger("death");
        playerMovement.enabled = false;
        playerRigid.linearVelocity = Vector2.zero;
    }

    private IEnumerator KnockbackCoroutine()
    {
        playerMovement.enabled = false;
        playerRigid.linearVelocity = Vector2.zero;
        playerAttack.EndCombo();
        yield return new WaitForSeconds(hitTime);

        if (!isDeath)
        {
            playerMovement.enabled = true;
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        float blinkInterval = 0.1f; // thời gian nháy
        float elapsed = 0f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            yield break;
        }

        while (elapsed < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;

        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
