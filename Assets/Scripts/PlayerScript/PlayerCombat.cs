using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    // enum ATTACKSTAGE
    // {
    //     None,
    //     Attack1,
    //     Attack2
    // }
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] PlayerInputController inputController;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerHealth playerHealth;

    [Header("SFX Settings")]
    [SerializeField] AudioClip sword1SFX;
    [SerializeField] AudioClip sword2SFX;
    [SerializeField] AudioClip swordlongSFX;
    [SerializeField] AudioSource audioSource;

    [Header("Attack Settings")]
    [SerializeField] BoxCollider2D swordHitbox;
    [SerializeField] PolygonCollider2D jumpSwordAttack;
    [SerializeField] AttackData swordAttackData;
    [SerializeField] float comboResetTime = 0.8f;

    [Header("Skill Cooldown")]
    [SerializeField] private float healCooldown = 3f;
    private float CooldownTimerskill = 0f;

    //ATTACKSTAGE currentStage = ATTACKSTAGE.None;
    Rigidbody2D rb;
    float comboTimer;
    private bool hasHitEnemy = false;
    private int atkcomboStage = 0;
    private int atkjumpcomboStage = 0;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (inputController == null)
            inputController = GetComponent<PlayerInputController>();

        if (animationController == null)
            animationController = GetComponent<PlayerAnimationController>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        comboTimer -= Time.deltaTime;
        CooldownTimerskill -= Time.deltaTime;

        if (comboTimer <= 0f)
        {
            atkcomboStage = 0;
            atkjumpcomboStage = 0;
        }

        if (inputController.Attack.WasPressedThisFrame())
        {
            if (playerMovement.IsGrounded())
            {
                Attack();
            }
            else
            {
                JumpAttack();
            }
        }
        else if (inputController.SpellAction.WasPressedThisFrame())
        {
            if (playerMovement.IsGrounded())
            {
                HealingSpell();
            }

        }
    }

    void Attack()
    {
        if (atkcomboStage < 2)
        {
            atkcomboStage++;
        }
        else
        {
            atkcomboStage = 1;
        }
        animationController.SetAttack(atkcomboStage);
        comboTimer = comboResetTime;
    }

    void JumpAttack()
    {
        if (atkjumpcomboStage < 2)
        {
            atkjumpcomboStage++;
        }
        else
        {
            atkjumpcomboStage = 2;
        }

        animationController.SetJumpAttack(atkjumpcomboStage);
        comboTimer = comboResetTime;
    }
    void EnableHitbox()
    {
        swordHitbox.enabled = true;
        hasHitEnemy = false;
    }

    void EnableJumpHitbox()
    {
        jumpSwordAttack.enabled = true;
        hasHitEnemy = false;
    }

    void DisableHitbox()
    {
        swordHitbox.enabled = false;
        jumpSwordAttack.enabled = false;
    }


    void HealingSpell()
    {
        if (CooldownTimerskill > 0f)
        {
            Debug.Log("Healing on cooldown!");
            return;
        }

        if (playerHealth.HealthPoint < playerHealth.maxHealthPoint)
        {
            animationController.CastHealingSpell();
            playerHealth.HealthPoint += 2;
            GlobalEffect.Instance.RegenEffect(transform.position + Vector3.up * 0.3f, transform);
            GlobalEffect.Instance.PlayHealSFX();
            StartCoroutine(DisableMovement());

            // Clamp biar gak lebih dari max
            if (playerHealth.HealthPoint > playerHealth.maxHealthPoint)
            {
                playerHealth.HealthPoint = playerHealth.maxHealthPoint;
            }

            CooldownTimerskill = healCooldown;
        }
    }

    private IEnumerator DisableMovement()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        movement.enabled = false;

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        movement.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitEnemy) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;

        AttackData activeAttack = null;

        if (swordHitbox.enabled)
            activeAttack = swordAttackData;
        else if (jumpSwordAttack.enabled)
            activeAttack = swordAttackData;

        if (activeAttack != null)
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(activeAttack.damageAmount);
                hasHitEnemy = true;
                Debug.Log($"Hit enemy {enemy.EnemyName} with {activeAttack.attackName} → {activeAttack.damageAmount} dmg");
            }
        }
    }


    public void Sword1SFX()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(sword1SFX);
        }
    }
    public void Sword2SFX()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(sword2SFX);
        }
    }

    public void SwordlongSFX()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(swordlongSFX);
        }
    }
}
