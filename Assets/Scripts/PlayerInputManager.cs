using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
    private Player _player;

    private PlayerInputActions _playerInputActions;

    private Camera _mainCamera;
    private Vector3 _moveDirection;

    private Vector2 _mouseDelta;

    private float _isFiring;
    private float _isAiming;

    private bool _canRotateMouse = false;

    private Weapon _equippedWeapon;

    [Header("PlayerMovementProperties")]
    private float _currentRotationVelocity;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _mouseSensitivity = 5.0f;
    [SerializeField] private float _maxRotationSpeed = 0.1f;

    [Header("WeaponProperties")]
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
        _player = GetComponent<Player>();
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
/*        if (_isFiring != 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(_crosshair.transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                FireWeapon(hit.point);
            else
                FireWeapon(_equippedWeapon.SpawnPoint.position + (ray.direction * _equippedWeapon.WeaponData._range));
        }*/
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

        if (_isAiming != 0)
        {
            _aimVirtualCamera.gameObject.SetActive(true);
            _thirdPersonVirtualCamera.gameObject.SetActive(false);
        }

        else
        {
            _aimVirtualCamera.gameObject.SetActive(false);
            _thirdPersonVirtualCamera.gameObject.SetActive(true);
        }

        if (dir.magnitude > 0.01f)
        {
            float angle = RotateTransform(dir);
            float smoothAngle = Mathf.SmoothDampAngle(_player.GFX.transform.localEulerAngles.y, angle, ref _currentRotationVelocity, _maxRotationSpeed);
            _player.GFX.transform.localEulerAngles = new Vector3(0.0f, smoothAngle, 0.0f);
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

        _canRotateMouse = false;

        _playerInputActions.Disable();

        _playerInputActions = null;
    }
    #endregion
}
