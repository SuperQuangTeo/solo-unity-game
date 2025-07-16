using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int numberOfHealth = 5;
    [SerializeField] private int currentHealth;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private float hitTime = 0.1f;
    [SerializeField] private bool isDeath = false;

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
        currentHealth = numberOfHealth;
        CreateHeart();
    }


    void Update()
    {
        updateHeartUI();

        if (currentHealth <= 0 && !isDeath)
        {
            Death();
        }
    }

    void CreateHeart()
    {
        for (int i = 0; i < numberOfHealth; i++)
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
            if (i < currentHealth)
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
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            _animator.SetTrigger("hit");
            StartCoroutine(KnockbackCoroutine());
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
}
