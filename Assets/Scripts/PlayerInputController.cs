using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction move;
    private InputAction jump;

    public Vector2 Movement { get; private set; }
    public InputAction JumpAction => jump;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");
        move = playerMap.FindAction("Move");
        jump = playerMap.FindAction("Jump");

        move.performed += ctx => Movement = ctx.ReadValue<Vector2>();
        move.canceled += ctx => Movement = Vector2.zero;
    }
}
