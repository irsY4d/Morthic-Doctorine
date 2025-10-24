using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // public Rigidbody2D.SlideMovement slideMovement = new Rigidbody2D.SlideMovement();
    // public Rigidbody2D.SlideResults SlideResults;

    [Header("Movement")]
    [SerializeField] float speed = 0f;
    [SerializeField] float jumpSpeed = 1f;

    [Header("Ground Check (Feet Collider)")]
    [SerializeField] BoxCollider2D feetCollider;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] float maxGroundAngle = 15f;

    [Header("Gravity Scale")]
    [SerializeField] float normalGravityScale = 1f;
    [SerializeField] float fallGravityScale = 2.5f;
    [SerializeField] float jumpCutGravityScale = 5f; // gravitasi saat tombol dilepas

    [Header("Reference")]
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] PlayerInputController inputController;
    public float Speed => speed;
    Rigidbody2D rb;
    Vector2 movement;
    bool isGrounded;
    bool isJumping = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Run(); // Update Movement Visual

        Jump();

        if (rb.linearVelocity.y > 0f && !inputController.JumpAction.IsPressed())
        {
            rb.gravityScale = jumpCutGravityScale;
            isJumping = false;
        }
        else if (rb.linearVelocity.y < -0.1f && !IsGrounded())
        {
            rb.gravityScale = fallGravityScale;
            animationController.Falling(true);
            isJumping = true;
        }
        else
        {
            rb.gravityScale = normalGravityScale;
            animationController.Falling(false);
        }
    }

    public void Run()
    {
        Vector2 move = inputController.Movement;
        float xMove = move.x;

        // Gerak horizontal
        rb.linearVelocity = new Vector2(xMove * speed, rb.linearVelocity.y);

        // Flip sprite
        if (xMove != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(xMove), 1, 1);
        }

        // Animasi jalan
        animationController.SetMoving(Mathf.Abs(xMove));
    }


    public void Jump()
    {
        // Cek input dulu sebelum ubah animasi
        if (inputController.JumpAction.WasPressedThisFrame() && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            animationController.SetTriggerJump();
            isJumping = true;
        }
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            return angle < maxGroundAngle;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
    }
}
