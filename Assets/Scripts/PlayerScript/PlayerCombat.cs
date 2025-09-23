using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    enum ATTACKSTAGE
    {
        None,
        Attack1,
        Attack2
    }
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] PlayerInputController inputController;
    [SerializeField] PlayerMovement playerMovement;

    [Header("Attack Settings")]
    [SerializeField] BoxCollider2D swordHitbox;
    [SerializeField] PolygonCollider2D jumpSwordAttack;
    [SerializeField] AttackData swordAttackData;
    [SerializeField] float comboResetTime = 0.8f;

    ATTACKSTAGE currentStage = ATTACKSTAGE.None;
    float comboTimer;
    private bool hasHitEnemy = false;


    void Update()
    {
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
            currentStage = ATTACKSTAGE.None;

        if (inputController.Attack.WasPressedThisFrame())
        {
            if (playerMovement.IsGrounded())
                Attack();
            else
            {
                JumpAttack();
            }
        }
    }

    void Attack()
    {
        switch (currentStage)
        {
            case ATTACKSTAGE.None:
                currentStage = ATTACKSTAGE.Attack1;
                //animationController.SetAttack();
                break;
            case ATTACKSTAGE.Attack1:
                currentStage = ATTACKSTAGE.Attack2;
                //animationController.SetAttack();
                break;
            default:
                currentStage = ATTACKSTAGE.None;
                break;
        }

        animationController.SetAttack();
        comboTimer = comboResetTime;
    }

    void JumpAttack()
    {
        switch (currentStage)
        {
            case ATTACKSTAGE.None:
                currentStage = ATTACKSTAGE.Attack1;
                break;
            case ATTACKSTAGE.Attack1:
                currentStage = ATTACKSTAGE.Attack2;
                break;
            default:
                currentStage = ATTACKSTAGE.None;
                break;
        }

        animationController.SetJumpAttack();
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
                Debug.Log($"Hit enemy with {activeAttack.attackName} → {activeAttack.damageAmount} dmg");
            }
        }
    }

}
