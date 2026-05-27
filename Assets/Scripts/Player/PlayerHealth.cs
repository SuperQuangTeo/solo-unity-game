using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, ISaveable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private float hitTime = 0.1f;
    [SerializeField] private bool isDeath = false;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float invincibleTime = 1f;
    [SerializeField] private float healTime = 1f;
    [SerializeField] private float healTimer = 0f;
    [SerializeField] private float respawnTime = 3f;

    private bool isStanding = false;
    [SerializeField] private bool isHealing = false;

    private int originalLayer;

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
    public bool IsDeath => isDeath;
    private HealingEffect healEffect;

    private bool IsStanding => Mathf.Abs(playerRigid.linearVelocity.x) < 0.1f && Mathf.Abs(playerRigid.linearVelocity.y) < 0.1f;
    private bool CanStartHealing =>
        IsStanding &&
        currentHeart < numberOfHeart &&
        playerInventory.heart > 0 &&
        playerMovement.IsGround() &&
        !playerMovement.isRolling &&
        !playerAttack.isAttacking &&
        !isDeath;

    public static event Action OnPlayerDeath;

    private void Awake()
    {
        playerMovement = GetComponent<movement>();
        playerAttack = GetComponent<Attack>();
        playerRigid = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<PlayerInventory>();
        originalLayer = gameObject.layer;
    }

    void Start()
    {
        healEffect = EffectManager.Instance.GetEffect<HealingEffect>();
        healEffect.StopEffect();
        CreateHeart();
    }


    void Update()
    {
        isStanding = Math.Abs(playerRigid.linearVelocity.x) < 0.01f && Math.Abs(playerRigid.linearVelocity.y) < 0.01f;
        updateHeartUI();

        if (currentHeart <= 0 && !isDeath)
        {
            Death();
        }

        HandleHealingInput();
    }

    private void HandleHealingInput()
    {
        if (Input.GetKeyDown(KeyCode.H) && CanStartHealing)
        {
            StartHealing();
        }

        if (isHealing)
        {
            if (!IsStanding || playerMovement.isRolling || playerAttack.isAttacking || !playerMovement.IsGround())
            {
                StopHealing();
                return;
            }

            healTimer += Time.deltaTime;
            if (healTimer >= healTime)
            {
                CompleteHealTick();
            }
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            StopHealing();
        }
    }

    private void StartHealing()
    {
        isHealing = true;
        healTimer = 0f;
        healEffect?.StartEffect(transform);
        AudioManager.Instance.PlaySFX("PlayerRegenHealth");
    }

    private void StopHealing()
    {
        if (!isHealing) return;

        isHealing = false;
        healTimer = 0f;
        healEffect?.StopEffect();
        AudioManager.Instance.Stop("PlayerRegenHealth");
    }

    private void CompleteHealTick()
    {
        UseHeart(1);
        StopHealing();
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
            if(currentHeart > 1)
            {
                AudioManager.Instance.PlaySFX("PlayerHit");
            }

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
        if(EnemySpawnerByWave.Instance != null && EnemySpawnerByWave.Instance.IsRoomChallengeActive)
        {
            EnemySpawnerByWave.Instance.ResetSpawner();
            
        }
        isDeath = true;
        _animator.SetTrigger("death");
        playerMovement.enabled = false;
        playerRigid.linearVelocity = Vector2.zero;
        gameObject.layer = LayerMask.NameToLayer("Ghost");

        OnPlayerDeath?.Invoke();

        AudioManager.Instance.PlaySFX("PlayerDeath");

        StartCoroutine(WaitToRespawn());
    }
    public void ResetPlayerAfterDeath()
    {
        isDeath = false;
        currentHeart = numberOfHeart;
        gameObject.layer = originalLayer;

        _animator.Rebind();
        _animator.Update(0f); 

        playerMovement.enabled = true;
        playerAttack.enabled = true; 
        playerRigid.simulated = true; 

        updateHeartUI();
    }

    public void SaveData(ref GameData data)
    {
        data.currentHeart = this.currentHeart;
    }

    public void LoadData(GameData data)
    {
        this.currentHeart = data.currentHeart;
        updateHeartUI();
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

    private IEnumerator WaitToRespawn()
    {
        yield return new WaitForSeconds(respawnTime);
        GameManager.Instance.RespawnPlayer();
        ResetPlayerAfterDeath();
    }
}
