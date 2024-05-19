using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : State
{
    private float t = 0.0f;

    public Walk(CharacterController characterController, Animator animator, PlayerInput playerInput) : base(characterController, animator, playerInput)
    {
        state = STATE.WALK;
    }

    public override void Begin()
    {
        base.Begin();
    }

    public override void Update()
    {
        base.Update();

        float value = Mathf.Lerp(0.0f, 1.0f, t);

        if(t < 1.0f)
            t += 3f * Time.deltaTime;

        _characterController.Move(WorldToLocalDir(_playerInput) * _playerInput.WalkSpeed * value * Time.deltaTime);
        _animator.SetFloat(GlobalVariables.WalkThreshold, value);

        if(_playerInput.MovementVector.magnitude <= 0.0f)
        {
            _nextState = new Idle(_characterController, _animator, _playerInput);
            stage = STAGE.END;
        }
    }

    public override void End()
    {
        base.End();
    }
}
