using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    private float t = 0.0f;

    public Idle(CharacterController character, Animator animator, PlayerInput playerInput) : base(character, animator, playerInput)
    {
        state = STATE.IDLE;
    }

    public override void Begin()
    {
        base.Begin();
    }

    public override void Update()
    {
        base.Update();

        if (_playerInput.MovementVector.magnitude <= 0.0f)
        {
            float value = Mathf.Lerp(1.0f, 0.0f, t);

            if (t < 1.0f)
                t += 3.0f * Time.deltaTime;

            _characterController.Move(WorldToLocalDir(_playerInput) * _playerInput.WalkSpeed * value * Time.deltaTime);
            _animator.SetFloat(GlobalVariables.WalkThreshold, value);
        }

        if (_playerInput.MovementVector.magnitude > 0.0f)
        {
            _nextState = new Walk(_characterController, _animator, _playerInput);
            stage = STAGE.END;
        }
    }

    public override void End()
    {
        base.End();
    }
}
