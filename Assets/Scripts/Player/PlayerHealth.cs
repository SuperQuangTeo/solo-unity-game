using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int numberOfHeart = 5;
    [SerializeField] private int currentHeart;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private float hitTime = 0.1f;
    [SerializeField] private bool isDeath = false;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float invincibleTime = 1f;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public GameObject heartPrefab;
    public Transform heartPanel;
    public List<Image> hearts = new List<Image>();

    public movement playerMovement;
    public Attack playerAttack;
    private void Awake()
    {
        playerMovement = GetComponent<movement>();
        playerAttack = GetComponent<Attack>();
        playerRigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHeart = numberOfHeart;
        CreateHeart();
    }


    void Update()
    {
        updateHeartUI();

        if (currentHeart <= 0 && !isDeath)
        {
            Death();
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
        if(currentHeart < numberOfHeart)
        {
            currentHeart += heartNumber;
        }
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
