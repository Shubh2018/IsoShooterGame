using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInput _input;

    private State _currentState;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;

    [Header("Walk Settings")]
    [SerializeField] private float _walkSpeed = 5.0f;

    [Header("UI Helper")]
    [SerializeField] private TMP_Text _currentStateText;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.InitializeActions();

        _input.WalkSpeed = _walkSpeed;
        _input.MainCamera = Camera.main;
    }

    private void Start()
    {
        _currentState = new Idle(_characterController, _animator, _input);
    }

    private void Update()
    {
        _currentStateText.text = $"State: {_currentState.state}";
        _currentState = _currentState.Process();
    }
}
