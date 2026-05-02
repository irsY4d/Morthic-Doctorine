using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction move;
    private InputAction jump;
    private InputAction attack;
    private InputAction spell;
    private InputAction interaction;
    

    public Vector2 Movement { get; private set; }
    public InputAction Move => move;
    public InputAction JumpAction => jump;
    public InputAction Attack => attack;
    public InputAction SpellAction => spell;
    public InputAction InteractionAction => interaction;

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
        var playerMap = inputActions.FindActionMap("Player", true);
        move = playerMap.FindAction("Move", true);
        jump = playerMap.FindAction("Jump", true);
        attack = playerMap.FindAction("Attack", true);
        spell = playerMap.FindAction("Spell", true);
        interaction = playerMap.FindAction("Interact", true);

        move.performed += ctx => Movement = ctx.ReadValue<Vector2>();
        move.canceled += ctx => Movement = Vector2.zero;
    }
}
