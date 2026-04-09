using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueData dialogueData; // 🔹 bukan DialogueLine[]

    [Header("UI Interaction Prompt")]
    [SerializeField] private GameObject interactUI;

    private bool playerIsClose;
    private PlayerInteraction playerInteraction;
    private Animator npcAnimator;
    private bool isTalking = false;

    void Start()
    {
        npcAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerIsClose && playerInteraction != null)
        {
            if (playerInteraction.TryInteract())
            {
                if (!isTalking)
                {
                    // 🔹 Mulai dialog
                    npcAnimator.SetBool("isTalking", true);
                    npcAnimator.SetTrigger("TriggerTalk");
                    interactUI.SetActive(false);

                    DialogueManager.Instance.StartDialogue(dialogueData, this);
                    isTalking = true;
                }
                else
                {
                    // 🔹 Saat sedang bicara dan pemain tekan lagi, lanjut ke line berikut
                    DialogueManager.Instance.NextLine();
                    npcAnimator.SetTrigger("TriggerTalk");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
            playerInteraction = collision.GetComponent<PlayerInteraction>();
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            interactUI.SetActive(false);

            if (isTalking)
            {
                EndDialogueAnimation();
            }
        }
    }

    public void EndDialogueAnimation()
    {
        npcAnimator.SetBool("isTalking", false);
        isTalking = false;
    }
}
