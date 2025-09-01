using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 0f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PlayerAnimationController animationController;
    Rigidbody2D rb;
    Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (movement.x != 0)
            spriteRenderer.flipX = movement.x < 0;

         animationController.SetMoving(movement.x != 0);
    }


    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
    }
}
