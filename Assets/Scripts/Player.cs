using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _cameraFollow;
    public Transform CameraFollow { get { return _cameraFollow; } }

    private Transform _aimCamera;
    public Transform AimCamera { get { return _aimCamera; } }

    [SerializeField] private Transform _gfx;
    public Transform GFX { get { return _gfx; } }

    [SerializeField] private Transform _groundCheck;
    public Transform GroundCheck { get { return _groundCheck; } }

    [SerializeField] private Animator _characterAnimator;

    public void SwitchMovementState(int blendstate)
    {
        _characterAnimator.SetFloat(GlobalVariables.WalkThreshold, blendstate);
    }
}
