using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

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

    public void SetAttack()
    {
        animator.SetTrigger("isAttack");
    }

    public void GetHit()
    {
        animator.SetTrigger("gotHit");
    }

    public void SetDeath()
    {
        animator.SetTrigger("isDeath");
    }

    public void SetJumpAttack()
    {
        animator.SetTrigger("JumpAttack");
    }
}
