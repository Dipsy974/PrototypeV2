using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class JumpState : BaseState
{
    public JumpState(StM_PlayerController player, CharacterControlsInput input) : base(player, input) { }

    private bool _initialJump;

    
    public override void OnEnter()
    {
        
    }
    
    public override void FixedUpdate()
    {
        _playerController.HandleRotation();
        HandleJump();
        OnJump();
        _playerController.PlayerMove();
    }
    
    public override void OnExit()
    {
        _playerController.PlayerFallTimer.Start();
        _playerController.CoyoteTimeCounter.Stop();
    }
    
    private void InitialJump()
    {
        _playerController.PlayerMoveInputY = _playerController.InitialJumpForce;
    }
    
    private void HandleJump()
    {
        var calculatedJumpInput = _playerController.PlayerMoveInputY;
        if (_initialJump)
        {
            calculatedJumpInput = _playerController.InitialJumpForce;
            _initialJump = false;
        }
        else
        {
            calculatedJumpInput = _playerController.InitialJumpForce * _playerController.ContinualJumpForceMultiplier;
        }
        _playerController.PlayerMoveInputY =  calculatedJumpInput; 
    }
    
    void OnJump()
    {
        if (!_input.JumpIsPressed)
        {
            _playerController.JumpTimer.Stop();
        }
    }
}
