using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    [SerializeField] LayerMask stairsLayer;
    private Rigidbody2D rb;
    private PlayerInputController input;
    private PlayerMovement movement;
    private PlayerAnimationController animController;

    private bool onStairs = false;
    private float normalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputController>();
        movement = GetComponent<PlayerMovement>();
        animController = GetComponent<PlayerAnimationController>();

        normalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (!onStairs) return;

        Vector2 move = input.Movement;

        // Gerak di tangga (pakai speed dari PlayerMovement atau stairsMoveSpeed)
        rb.linearVelocity = new Vector2(move.x * movement.Speed, move.y * movement.Speed);

        // Set nilai ke animasi
        float moveMagnitude = move.magnitude;
        animController.SetMoving(moveMagnitude);

        // Kalau tidak bergerak → berhenti
        if (moveMagnitude < 0.05f)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private bool IsStairs(Collider2D other)
    {
        return ((1 << other.gameObject.layer) & stairsLayer) != 0 || other.CompareTag("Stairs");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsStairs(collision))
        {
            onStairs = true;
            rb.gravityScale = 0f; // Matikan gravitasi total
            rb.linearVelocity = Vector2.zero;
            movement.enabled = false; // Disable kontrol movement normal
            animController.SetMoving(0f);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (IsStairs(collision))
        {
            onStairs = false;
            rb.gravityScale = normalGravity;
            movement.enabled = true;
            animController.SetMoving(0f); // Pastikan animasi kembali ke idle
        }
    }
}
