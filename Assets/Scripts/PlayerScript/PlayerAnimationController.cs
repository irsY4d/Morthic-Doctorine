using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private bool isDead = false;

    public void SetMoving(float isMoving)
    {
        //animator.SetBool("isMoving", isMoving);
        animator.SetFloat("xMove", isMoving);
    }

    public void SetTriggerJump()
    {
        animator.SetTrigger("isJump");
    }

    public void Falling(bool isFalling)
    {
        animator.SetBool("isFalling", isFalling);
    }

    public void SetAttack(int stage)
    {
        animator.SetInteger("comboStage", stage);
        animator.SetTrigger("isAttack");
    }

    public void GetHit()
    {
        if (isDead)
        {
            return;
        }
        animator.SetTrigger("gotHit");
    }

    public void SetDeath(bool hasDead)
    {
        isDead = hasDead;
        animator.SetBool("hasDead", hasDead);
    }

    public void SetJumpAttack(int stage)
    {
        animator.SetInteger("comboStage", stage);
        animator.SetTrigger("JumpAttack");
    }

    public void CastHealingSpell()
    {
        animator.SetTrigger("castHealing");
    }
}
