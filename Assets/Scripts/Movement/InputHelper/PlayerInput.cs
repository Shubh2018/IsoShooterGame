using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput
{
    private PlayerInputActions _playerInputActions;

    public Vector2 MovementVector { get; private set; }
    public Vector2 LookVector { get; private set; }

    public float WalkSpeed { get; set; }

    public Camera MainCamera { get; set; }

    public void InitializeActions()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Enable();

        _playerInputActions.Player.Move.performed += PlayerMove;
        _playerInputActions.Player.Move.canceled += PlayerMove;

        _playerInputActions.Player.Look.performed += PlayerLook;
        _playerInputActions.Player.Look.canceled += PlayerLook;
    }

    public void DeInitializeActions()
    {
        _playerInputActions.Player.Move.performed -= PlayerMove;
        _playerInputActions.Player.Move.canceled -= PlayerMove;

        _playerInputActions.Player.Look.performed -= PlayerLook;
        _playerInputActions.Player.Look.canceled -= PlayerLook;

        _playerInputActions.Disable();
    }

    private void PlayerMove(InputAction.CallbackContext context)
    {
        MovementVector = context.ReadValue<Vector2>();
    }

    private void PlayerLook(InputAction.CallbackContext context) 
    { 
        LookVector = context.ReadValue<Vector2>();
    }
}
