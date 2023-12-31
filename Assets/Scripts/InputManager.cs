using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Player _player;

    private PlayerInputActions _playerInputActions;

    private Vector2 _pressedDirection;
    private Vector3 _moveDirection;

    private Vector2 _mouseDelta;
    private Ray _mouseRay;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _mouseSensitivity = 5.0f;

    private void Awake()
    {
        _player = GetComponent<Player>(); 
        _playerInputActions = new PlayerInputActions();
    }

    private void Update()
    {
        transform.Translate(_moveDirection.normalized * _speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if(Physics.Raycast(_mouseRay, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 lookDir = hit.point;
            lookDir.y = _player.GFX.transform.position.y;
            _player.GFX.transform.LookAt(lookDir);

            Vector3 rotation = _player.GFX.transform.localEulerAngles;
            rotation.y -= 90.0f;
            _player.GFX.transform.localEulerAngles = rotation;
        }
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();

        _playerInputActions.Player.Move.performed += OnMove;
        _playerInputActions.Player.Move.canceled += OnMove;

        _playerInputActions.Player.Look.performed += OnLook;
        _playerInputActions.Player.Look.canceled += OnLook;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Move.performed -= OnMove;
        _playerInputActions.Player.Move.canceled -= OnMove;

        _playerInputActions.Player.Look.performed -= OnLook;
        _playerInputActions.Player.Look.canceled -= OnLook;

        _playerInputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _pressedDirection = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(_pressedDirection.x, 0.0f, _pressedDirection.y);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
        _mouseRay = Camera.main.ScreenPointToRay(_mouseDelta);
    }
}
