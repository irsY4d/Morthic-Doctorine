using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 0f;
    [SerializeField] float jumpSpeed = 1f;

    [Header("Ground Check (Feet Collider)")]
    [SerializeField] BoxCollider2D feetCollider;
    [SerializeField] LayerMask groundLayer;

    [Header("Gravity Scale")]
    [SerializeField] float normalGravityScale = 1f;
    [SerializeField] float fallGravityScale = 2.5f;
    [SerializeField] float jumpCutGravityScale = 5f; // gravitasi saat tombol dilepas


    [Header("Reference")]
    //[SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] PlayerInputController inputController;
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
        movement = inputController.Movement;

        if (movement.x != 0)
        {
            // flip seluruh GameObject (sprite + collider + child)
            transform.localScale = new Vector3(movement.x < 0 ? -1 : 1, 1, 1);
        }

        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
        animationController.SetMoving(Mathf.Abs(movement.x));
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
        return isGrounded = feetCollider.IsTouchingLayers(groundLayer);
    }
}
