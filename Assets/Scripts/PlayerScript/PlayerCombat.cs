using UnityEngine;

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

    [Header("Attack Settings")]
    [SerializeField] BoxCollider2D swordHitbox;
    [SerializeField] PolygonCollider2D jumpSwordAttack;
    [SerializeField] AttackData swordAttackData;
    [SerializeField] float comboResetTime = 0.8f;

    //ATTACKSTAGE currentStage = ATTACKSTAGE.None;
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
    }
    
    void Update()
    {
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
        {
            atkcomboStage = 0;
            atkjumpcomboStage = 0;
        }

        if (inputController.Attack.WasPressedThisFrame())
        {
            if (playerMovement.IsGrounded())
                Attack();
            else
            {
                JumpAttack();
            }
        }
        else if (inputController.SpellAction.WasCompletedThisFrame())
        {
            HealingSpell();
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
        if (playerHealth.HealthPoint < playerHealth.maxHealthPoint)
        {
            animationController.CastHealingSpell();
            playerHealth.HealthPoint += 2;

            // Clamp biar gak lebih dari max
            if (playerHealth.HealthPoint > playerHealth.maxHealthPoint)
            {
                playerHealth.HealthPoint = playerHealth.maxHealthPoint;
            }
        }
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

}
