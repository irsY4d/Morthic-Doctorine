using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void SetMoving(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    public void SetTriggerJump()
    {
        animator.SetTrigger("isJump");
    }

    public void Falling(bool isFalling)
    {
        animator.SetBool("isFalling", isFalling);
    }

}
