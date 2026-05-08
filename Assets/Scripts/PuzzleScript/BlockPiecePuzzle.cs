using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UI;

public class BlockPiecePuzzle : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private Transform slotsParent;
    private RectTransform[] slots;

    [Header("Blocks")]
    [SerializeField] RectTransform redBlock;
    [SerializeField] RectTransform blueBlock;
    [SerializeField] RectTransform greenBlock;

    [Header("Input Asset")]
    [SerializeField] private InputActionAsset controls;

    [Header("Reference Puzzel Solved")]
    [SerializeField] private PuzzleButton puzzleButton;
    [SerializeField] private OpenPath openPath;

    //ENUM Puzzle Mode
    private enum PuzzleMode
    {
        Select,
        Move
    }
    private PuzzleMode currentMode = PuzzleMode.Select;

    // Block Index
    private int redIndex = 0;
    private int blueIndex = 4;
    private int greenIndex = 8;

    //Selected Block
    private RectTransform selectedBlock;
    private int selectedIndex;
    private int previewIndex;

    // Input Actions  
    private InputAction moveAction;
    private InputAction puzzleComfirmAction;
    //private InputAction puzzleCancelAction;

    //Time Controller for stering input, so the player can't spam input and break the puzzle
    private float inputCooldown = 0.2f;
    private float timer;

    //State Solved puzzle
    private bool isPuzzleSolved = false;
    public bool puzzleSolved => isPuzzleSolved;


    void Awake()
    {
        // Get all slots from the parent
        int count = slotsParent.childCount;
        slots = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            slots[i] = slotsParent.GetChild(i).GetComponent<RectTransform>();
        }

        var puzzleMap = controls.FindActionMap("Puzzle", true);
        moveAction = puzzleMap.FindAction("Move", true);
        puzzleComfirmAction = puzzleMap.FindAction("PuzzleConfirm", true);
        //puzzleCancelAction = puzzleMap.FindAction("PuzzleCancel", true);
    }

    void Start()
    {
        selectedBlock = redBlock;
        selectedIndex = redIndex;

        HighlightSelected();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        //CONFIRM MOVE and SELECTION
        if (puzzleComfirmAction.WasPressedThisFrame())
        {
            if (currentMode == PuzzleMode.Select)
            {
                currentMode = PuzzleMode.Move;
                previewIndex = selectedIndex;
            }
            else
            {
                ConfirmMove();
                currentMode = PuzzleMode.Select;
            }
        }

        //CANCEL MOVE
        // if (puzzleCancelAction.WasPressedThisFrame())
        // {
        //     if (currentMode == PuzzleMode.Move)
        //     {
        //         CancelMove();
        //         currentMode = PuzzleMode.Select;
        //     }
        // }

        if (timer > 0f)
            return;

        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.magnitude < 0.5f)
            return;

        timer = inputCooldown;

        //MOVE
        // if current mode is select then move selection, else move block
        if (currentMode == PuzzleMode.Select)
        {
            MoveSelection(input);
        }
        // else if current mode  is move then move block
        else
        {
            // move block to the direction of input
            MoveBlock(input);
        }
    }

    // -------------
    //SELECT MODE
    #region Select Mode
    private void MoveSelection(Vector2 input)
    {
        // select 3 block
        if (input.x > 0.5f || input.y < -0.5f)
        {
            // select next block
            SelectNextBlock();
        }
        else if (input.x < -0.5f || input.y > 0.5f)
        {
            // select previous block
            SelectPreviousBlock();
        }
    }

    void SelectNextBlock()
    {
        if (selectedBlock == redBlock)
        {
            selectedBlock = blueBlock;
            selectedIndex = blueIndex;
        }
        else if (selectedBlock == blueBlock)
        {
            selectedBlock = greenBlock;
            selectedIndex = greenIndex;
        }
        else if (selectedBlock == greenBlock)
        {
            selectedBlock = redBlock;
            selectedIndex = redIndex;
        }

        HighlightSelected();
    }

    void SelectPreviousBlock()
    {
        if (selectedBlock == redBlock)
        {
            selectedBlock = greenBlock;
            selectedIndex = greenIndex;
        }
        else if (selectedBlock == blueBlock)
        {
            selectedBlock = redBlock;
            selectedIndex = redIndex;
        }
        else if (selectedBlock == greenBlock)
        {
            selectedBlock = blueBlock;
            selectedIndex = blueIndex;
        }

        HighlightSelected();
    }

    void HighlightSelected()
    {
        // reset
        redBlock.localScale = Vector3.one;
        blueBlock.localScale = Vector3.one;
        greenBlock.localScale = Vector3.one;

        // highlight
        selectedBlock.localScale = Vector3.one * 1.1f;
    }
    #endregion

    // -------------
    //MOVE MODE
    #region Move Mode
    private void MoveBlock(Vector2 input)
    {
        // move block to the direction of input
        if (input.x > 0.5f)
        {
            // move right
            MoveBlockToDirection(Vector2.right);
        }
        else if (input.x < -0.5f)
        {
            // move left
            MoveBlockToDirection(Vector2.left);
        }
        else if (input.y > 0.5f)
        {
            // move up
            MoveBlockToDirection(Vector2.up);
        }
        else if (input.y < -0.5f)
        {
            // move down
            MoveBlockToDirection(Vector2.down);
        }
    }

    private void MoveBlockToDirection(Vector2 direction)
    {
        // calculate target index based on direction
        int targetIndex = previewIndex;

        if (direction == Vector2.right)
        {
            if (previewIndex % 3 == 2) return;
            targetIndex += 1;
        }
        else if (direction == Vector2.left)
        {
            if (previewIndex % 3 == 0) return;
            targetIndex -= 1;
        }
        else if (direction == Vector2.up)
        {
            if (previewIndex < 3) return;
            targetIndex -= 3;
        }
        else if (direction == Vector2.down)
        {
            if (previewIndex >= 6) return;
            targetIndex += 3;
        }

        if (IsSlotOccupied(targetIndex)) return;

        previewIndex = targetIndex;

        // move block to target slot
        selectedBlock.anchoredPosition = slots[previewIndex].anchoredPosition;
    }
    private void ConfirmMove()
    {
        selectedIndex = previewIndex;

        if (selectedBlock == redBlock)
        {
            redIndex = selectedIndex;
        }
        else if (selectedBlock == blueBlock)
        {
            blueIndex = selectedIndex;
        }

        else if (selectedBlock == greenBlock)
        {
            greenIndex = selectedIndex;
        }

        PuzzleSolved();
    }

    private bool IsSlotOccupied(int index)
    {
        if (selectedBlock == redBlock)
            return index == blueIndex || index == greenIndex;

        if (selectedBlock == blueBlock)
            return index == redIndex || index == greenIndex;

        if (selectedBlock == greenBlock)
            return index == redIndex || index == blueIndex;

        return false;
    }

    // private void CancelMove()
    // {
    //     previewIndex = selectedIndex;
    //     selectedBlock.anchoredPosition = slots[selectedIndex].anchoredPosition;
    // }

    #endregion

    // -------------
    //Check Puzzle Solved
    void PuzzleSolved()
    {
        if (isPuzzleSolved)
            return;

        if (redIndex == 8 && blueIndex == 5 && greenIndex == 6)
        {
            isPuzzleSolved = true;
            
            Debug.Log("Puzzle Solved!");
            StartCoroutine(PuzzleSolvedRoutine());
            Debug.Log(openPath);
            openPath.Open(); // open the path
        }
    }

    IEnumerator PuzzleSolvedRoutine()
    {
        // Force close the puzzle UI if it's still open
        puzzleButton.ForceClosePuzzle();

        yield return new WaitForSeconds(0.5f); // wait for 1 second

    }
}
