using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleButton : MonoBehaviour
{
    [Header("Puzzle UI")]
    [SerializeField] private GameObject interactUI;
    [SerializeField] private GameObject puzzleUI;

    //Input Assset
    [SerializeField] private InputActionAsset controls;

    //Reference Puzzle Solved
    [SerializeField] private BlockPiecePuzzle puzzle;

    //Input Actions Puzzle UI
    private InputAction cancelAction;

    private PlayerInteraction playerInteraction;
    private bool playerIsClose;

    void Awake()
    {
        var uiMap = controls.FindActionMap("UI", true);
        cancelAction = uiMap.FindAction("Cancel", true);
    }

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);

        if (puzzleUI != null)
            puzzleUI.SetActive(false);
    }

    void Update()
    {
        if (!playerIsClose || playerInteraction == null)
            return;

        if (playerInteraction.TryInteract())
        {
            if (interactUI != null && interactUI.activeSelf)
            {
                interactUI.SetActive(false);
                if (puzzleUI != null)
                    puzzleUI.SetActive(true);
            }
        }

        if (puzzleUI != null && puzzleUI.activeSelf && cancelAction.WasPressedThisFrame())
        {
            puzzleUI.SetActive(false);
            interactUI.SetActive(true);
            playerInteraction.EndInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInteraction = collision.GetComponent<PlayerInteraction>();
        playerIsClose = playerInteraction != null;

        if (playerIsClose && interactUI != null)
            interactUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerIsClose = false;

        if (interactUI != null)
            interactUI.SetActive(false);

        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        playerInteraction?.EndInteraction();
        playerInteraction = null;
    }

    public void ForceClosePuzzle()
    {
        //force close puzzle
        if (puzzleUI != null && puzzleUI.activeSelf)
        {
            puzzleUI.SetActive(false);
            if (interactUI != null)
                interactUI.SetActive(true);
            playerInteraction?.EndInteraction();
        }

        if (puzzle != null && puzzle.puzzleSolved)
        {
            DisablePuzzle();
        }
    }

    void DisablePuzzle()
    {
        if (interactUI != null)
            interactUI.SetActive(false);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        this.enabled = false;
    }
}
