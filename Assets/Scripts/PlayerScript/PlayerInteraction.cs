using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInputController inputController;
    private PlayerCombat playerCombat;

    public bool IsInteracting { get; private set; }

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        inputController = GetComponent<PlayerInputController>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    public bool TryInteract()
    {
        if (inputController.InteractionAction.WasPressedThisFrame())
        {
            IsInteracting = true;
            inputController.Move.Disable();  
            inputController.JumpAction.Disable();
            playerCombat.enabled = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f; // kalau ada rotasi
            }
            return true; // hanya true ketika pertama kali tombol ditekan
        }

        return false; // bukan tiap frame
    }

    public void EndInteraction()
    {
        IsInteracting = false;
        inputController.Move.Enable();  
        inputController.JumpAction.Enable();
        playerCombat.enabled = true;
    }
}
