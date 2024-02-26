using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{

    private PlayerInputActions _playerInputActions;

    private Camera _mainCamera;
    private Vector3 _moveDirection;

    private Vector2 _mouseDelta;

    private float _isFiring;
    private float _isAiming;
    private float _jumpPressed;

    private bool _isGrounded = true;

    private bool _canRotateMouse = false;

    private Weapon _equippedWeapon;

    [Header("Player Script")]
    [SerializeField] private Player _player;

    [Header("Player Movement Properties")]
    private float _currentRotationVelocity;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _mouseSensitivity = 5.0f;
    [SerializeField] private float _maxRotationSpeed = 0.1f;

    [SerializeField] private float _jumpTime = 1.0f;
    [SerializeField] private float _jumpSpeed = 5.0f;
    [SerializeField] private float _groundCheckDistance = 1.0f;

    [SerializeField] private Vector3 _halfExtents;
    [SerializeField] private LayerMask _jumpLayer;

    [Header("Weapon Properties")]
    [SerializeField] private Transform _weaponSocket;
    [SerializeField] private Image _crosshair;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _thirdPersonVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;

    public Action<Vector3> FireWeapon;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _equippedWeapon = _weaponSocket.GetComponentInChildren<Weapon>();

        _aimVirtualCamera.gameObject.SetActive(false);
        _thirdPersonVirtualCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        MovePlayer(_moveDirection);
    }

    private void FixedUpdate()
    {
        if (_isFiring != 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(_crosshair.transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                FireWeapon(hit.point);
            else
                FireWeapon(ray.direction * _equippedWeapon.WeaponData._range);
        }

        _isGrounded = Physics.OverlapBox(_player.GroundCheck.position, _halfExtents, Quaternion.identity, _jumpLayer) != null ? true : false;
        Debug.Log(_isGrounded);
    }

    private void LateUpdate()
    {
        MouseLook(_mouseDelta);
    }

    private void OnEnable()
    {
        InitializePlayerInput();
    }

    private void OnDisable()
    {
        DeinitializePlayerInput();
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        Vector3 dir = new Vector3(moveDirection.x, 0.0f, moveDirection.y);

        Vector3 localCameraDir = TransformGlobalToLocal(dir);
        localCameraDir.y = 0.0f;

        if(_isAiming == 0f)
        {
            ToggleAim(false);

            if (dir.magnitude > 0.01f || _isFiring != 0)
            {
                float angle = RotateTransform(dir);
                float smoothAngle = Mathf.SmoothDampAngle(_player.GFX.transform.localEulerAngles.y, angle, ref _currentRotationVelocity, _maxRotationSpeed);
                _player.GFX.transform.localEulerAngles = new Vector3(0.0f, smoothAngle, 0.0f);
            }
        }

        else
        {
            ToggleAim(true);
            _player.GFX.transform.localEulerAngles = _player.CameraFollow.transform.localEulerAngles;
        }
                
        transform.Translate(localCameraDir * _speed * Time.deltaTime);
    }

    private Vector3 TransformGlobalToLocal(Vector3 worldDir)
    {
        Vector3 localCameraX = worldDir.x * _mainCamera.transform.right;
        Vector3 localCameraZ = worldDir.z * _mainCamera.transform.forward;

        return localCameraX + localCameraZ;
    }

    private float RotateTransform(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
        return angle;
    }

    private void ToggleAim(bool aim)
    {
        _aimVirtualCamera.gameObject.SetActive(aim);
        _thirdPersonVirtualCamera.gameObject.SetActive(!aim);
    }

    private void MouseLook(Vector3 delta)
    {
        if (!_canRotateMouse) return;

        Vector3 camRotation = _player.CameraFollow.transform.localEulerAngles;

        camRotation.x += delta.y * _mouseSensitivity * Time.deltaTime;
        camRotation.y += delta.x * _mouseSensitivity * Time.deltaTime;

        if (camRotation.x > 180.0f && camRotation.x < 340.0f)
            camRotation.x = 340.0f;

        else if (camRotation.x < 180.0f && camRotation.x > 60.0f)
            camRotation.x = 60.0f;

        _player.CameraFollow.transform.localEulerAngles = camRotation;
    }

    #region InputCallbacks
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    private void OnFireHeld(InputAction.CallbackContext context)
    {
        _isFiring = context.ReadValue<float>();
    }

    private void OnFireLeft(InputAction.CallbackContext context)
    {
        _isFiring = 0.0f;
    }

    private void OnAimHeld(InputAction.CallbackContext context)
    {
        _isAiming = context.ReadValue<float>();
    }

    private void OnAimLeft(InputAction.CallbackContext context)
    {
        _isAiming = 0.0f;
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        _jumpPressed = context.ReadValue<float>();
    }

    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        _jumpPressed = 0.0f;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() != 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;   
        }
    }
    #endregion

    #region PlayerInputLoadFunctions
    private void InitializePlayerInput()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Enable();

        _playerInputActions.Player.Move.performed += OnMove;
        _playerInputActions.Player.Move.canceled += OnMove;

        _playerInputActions.Player.Look.performed += OnLook;
        _playerInputActions.Player.Look.canceled += OnLook;

        _playerInputActions.Player.Fire.performed += OnFireHeld;
        _playerInputActions.Player.Fire.canceled += OnFireLeft;

        _playerInputActions.Player.Pause.performed += OnPause;

        _playerInputActions.Player.Aim.performed += OnAimHeld;
        _playerInputActions.Player.Aim.canceled += OnAimLeft;

        _playerInputActions.Player.Jump.started += OnJumpPressed;
        _playerInputActions.Player.Jump.canceled += OnJumpReleased;

        _canRotateMouse = true;
    }

    private void DeinitializePlayerInput()
    {
        _playerInputActions.Player.Move.performed -= OnMove;
        _playerInputActions.Player.Move.canceled -= OnMove;

        _playerInputActions.Player.Look.performed -= OnLook;
        _playerInputActions.Player.Look.canceled -= OnLook;

        _playerInputActions.Player.Fire.performed -= OnFireHeld;
        _playerInputActions.Player.Fire.canceled -= OnFireLeft;

        _playerInputActions.Player.Pause.performed -= OnPause;

        _playerInputActions.Player.Aim.performed -= OnAimHeld;
        _playerInputActions.Player.Aim.canceled -= OnAimLeft;

        _playerInputActions.Player.Jump.started -= OnJumpPressed;
        _playerInputActions.Player.Jump.canceled -= OnJumpReleased;

        _canRotateMouse = false;

        _playerInputActions.Disable();

        _playerInputActions = null;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_player.GroundCheck.position, _halfExtents * 2);
    }
}
