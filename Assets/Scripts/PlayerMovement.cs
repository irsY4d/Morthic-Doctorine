using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 0f;
    [SerializeField] float jumpSpeed = 1f;

    [Header("Ground Check (Feet Collider)")]
    BoxCollider2D feetCollider;
    [SerializeField] LayerMask groundLayer;

    [Header("Reference")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] PlayerInputController inputController;
    Rigidbody2D rb;
    Vector2 movement;
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Run(); // Update Movement Visual
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y); //Update Movement Physics

        Jump();

        if (rb.linearVelocity.y < -0.1f && !isGrounded)
        {
            animationController.Falling(true);
        }
        else
        {
            animationController.Falling(false);
        }
    }

    void Run()
    {
        movement = inputController.Movement;

        if (movement.x != 0)
            spriteRenderer.flipX = movement.x < 0;

        animationController.SetMoving(movement.x != 0);
    }

    void Jump()
    {
        isGrounded = feetCollider.IsTouchingLayers(groundLayer);

        // Cek input dulu sebelum ubah animasi
        if (inputController.JumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            animationController.SetTriggerJump();
        }
    }
}
