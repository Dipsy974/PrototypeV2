using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class JumpState : BaseState
{
    public JumpState(StM_PlayerController player, StM_InputReader input) : base(player, input)
    {
        _input.Jump += OnJump;
        _playerController.JumpMinTimer.OnTimerStop += () => CheckMinJumpTime();
    }

    private bool _minTime;
    
    public override void OnEnter()
    {
        _playerController.JumpMinTimer.Start();
    }
    
    
    public override void FixedUpdate()
    {
        _playerController.HandleRotation();
        HandleJump();
        _playerController.PlayerMove();
    }
    
    public override void OnExit()
    {
        _playerController.PlayerFallTimer.Start();
        _playerController.CoyoteTimeCounter.Stop();
        _playerController.InitialJump = false;
        _minTime = false;
    }
    
    private void HandleJump()
    {
        var calculatedJumpInput = _playerController.PlayerMoveInputY;
        if (!_playerController.InitialJump)
        {
            calculatedJumpInput = _playerController.InitialJumpForce;
            _playerController.InitialJump = true;
        }
        else
        {
            calculatedJumpInput = _playerController.InitialJumpForce * _playerController.ContinualJumpForceMultiplier;
        }
        _playerController.PlayerMoveInputY =  calculatedJumpInput; 
    }
    
    void OnJump(bool jumpisPressed)
    {
        if (!jumpisPressed)
        {
            if(!_playerController.JumpMinTimer.IsRunning)
                _playerController.JumpTimer.Stop();
            else
            {
                _minTime = true;
            }
            
        }
    }

    private void CheckMinJumpTime()
    {
        if (_minTime)
        {
            _playerController.JumpTimer.Stop();
        }
    }
}
