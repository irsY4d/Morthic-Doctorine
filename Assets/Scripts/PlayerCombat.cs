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

    float comboTimer;
    [SerializeField] float comboResetTime = 0.8f;

    ATTACKSTAGE currentStage = ATTACKSTAGE.None;

    void Update()
    {
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
            currentStage = ATTACKSTAGE.None;

        if (inputController.Attack.WasPressedThisFrame())
        {
            if (playerMovement.IsGrounded())
                Attack();
        }
    }


    void Attack()
    {
        switch (currentStage)
        {
            case ATTACKSTAGE.None:
                currentStage = ATTACKSTAGE.Attack1;
                animationController.SetAttack();
                break;
            case ATTACKSTAGE.Attack1:
                currentStage = ATTACKSTAGE.Attack2;
                animationController.SetAttack();
                break;
            default:
                currentStage = ATTACKSTAGE.None;
                break;
        }

        comboTimer = comboResetTime;
    }

    void JumpAttack()
    {

    }
}
