using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void SetMoving(bool isMoving)
    { 
        animator.SetBool("isMoving", isMoving);
    }
}
