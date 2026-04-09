using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueText;

    [Header("Settings")]
    [SerializeField] private float wordSpeed = 0.03f;
    private PlayerInteraction playerInteraction;

    private DialogueData currentData;   // 🔹 satu data karakter + semua line
    private NPC activeNPC;
    private int lineIndex;              // 🔹 index line dalam DialogueData
    private bool isTyping;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
    }

    public void StartDialogue(DialogueData data, NPC npc)
    {
        currentData = data;
        activeNPC = npc;
        lineIndex = 0;

        dialoguePanel.SetActive(true);
        continueText.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        // 🔹 Set nama & icon karakter
        nameText.text = currentData.character.name;
        characterImage.sprite = currentData.character.icon;

        // 🔹 Ambil line sesuai index
        string line = currentData.lines[lineIndex].line;

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        continueText.SetActive(true);
    }

    public void NextLine()
    {
        if (isTyping) return;

        continueText.SetActive(false);

        if (lineIndex < currentData.lines.Length - 1)
        {
            lineIndex++;
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        activeNPC.EndDialogueAnimation();
        dialogueText.text = "";
        nameText.text = "";
        characterImage.sprite = null;

        playerInteraction.EndInteraction();
    }
}
