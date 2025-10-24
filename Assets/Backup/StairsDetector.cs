using UnityEngine;

public class StairsDetector : MonoBehaviour
{
    //[SerializeField] LayerMask stairsLayer;
    [SerializeField] LayerMask groundLayer;

    private Collider2D stairsCollider;
    private PlayerInputController playerInput;
    private BoxCollider2D playerFeet;
    private bool playerInside = false;

    void Start()
    {
        stairsCollider = GetComponent<Collider2D>();
        if (stairsCollider == null)
            Debug.LogError("StairsDetector: Collider not found!");
    }

    void Update()
    {
        if (!playerInside || playerInput == null || playerFeet == null) return;

        Vector2 move = playerInput.Movement;

        // ✅ Naik tangga — jika tekan W dan kaki sedang di tangga
        if (move.y > 0.1f && CompareTag("Stairs"))
        {
            stairsCollider.isTrigger = false;
        }
        // ✅ Kalau kaki sudah di tanah normal → tangga bisa dilewati lagi
        else if (playerFeet.IsTouchingLayers(groundLayer))
        {
            stairsCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerInput = other.GetComponent<PlayerInputController>();
            playerFeet = other.GetComponentInChildren<BoxCollider2D>(); // ambil feet collider dari child
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerInput = null;
            playerFeet = null;

            // biar aman — tangga kembali bisa dilewati
            if (stairsCollider != null)
                stairsCollider.isTrigger = true;
        }
    }
}
