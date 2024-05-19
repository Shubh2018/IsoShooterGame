using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public enum STATE { IDLE, WALK, SPRINT };

    public enum STAGE { BEGIN, UPDATE, END };

    public STATE state;
    public STAGE stage;

    protected CharacterController _characterController;
    protected Animator _animator;
    protected PlayerInput _playerInput;

    protected State _nextState;

    public State(CharacterController characterController, Animator animator, PlayerInput playerInput)
    {
        _characterController = characterController;
        _animator = animator;
        _playerInput = playerInput;
    }

    public virtual void Begin() { stage = STAGE.UPDATE; }
    public virtual void Update() { stage = STAGE.UPDATE; }
    public virtual void End() { stage = STAGE.END; }

    public State Process()
    {
        if (stage == STAGE.BEGIN) Begin();
        if (stage == STAGE.UPDATE) Update();
        if (stage == STAGE.END)
        {
            End();
            return _nextState;
        }

        return this;
    }

    protected Vector3 WorldToLocalDir(PlayerInput input)
    {
        Vector3 dir = new Vector3(input.MovementVector.x, 0.0f, input.MovementVector.y);

        Vector3 camX = input.MainCamera.transform.right * dir.x;
        Vector3 camZ = input.MainCamera.transform.forward * dir.z;

        Vector3 cameraLocalDir = camX + camZ;

        return cameraLocalDir.normalized;
    }
}
