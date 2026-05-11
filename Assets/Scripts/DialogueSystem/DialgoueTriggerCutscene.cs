using UnityEngine;

public class DialgoueTriggerCutscene : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private string cutsceneID;

    [Header("References")]
    [SerializeField] private NPC npc; // NPC yang dialognya akan dipicu
    private PlayerInputController inputController;
    private PlayerCombat playerCombat;
    private PlayerInteraction playerInteraction;

    private bool hasTriggered = false;
    private bool isCutsceneActive = false;

    void Start()
    {
        playerInteraction = FindAnyObjectByType<PlayerInteraction>();
        inputController = FindAnyObjectByType<PlayerInputController>();
        playerCombat = FindAnyObjectByType<PlayerCombat>();

        if (GameManager.Instance.IsCutsceneDone(cutsceneID))
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Cek input setiap frame HANYA jika cutscene sedang jalan
        if (isCutsceneActive)
        {
            if (inputController.InteractionAction.WasPressedThisFrame())
            {
                // Kita panggil NextLine melalui DialogueManager
                DialogueManager.Instance.NextLine();
            }
            GameManager.Instance.MarkCutsceneAsDone(cutsceneID);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered)
        {
            return; // Cegah trigger berulang
        }

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;
            isCutsceneActive = true;

            // Nonaktifkan kontrol pemain untuk cutscene
            inputController.Move.Disable();
            inputController.JumpAction.Disable();
            playerCombat.enabled = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f; // kalau ada rotasi
            }

            // 🔹 Trigger cutscene dialog
            DialogueManager.Instance.StartDialogue(dialogueData, npc);
        }
    }
}
