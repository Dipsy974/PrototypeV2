using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : BaseState
{
    public GroundedState(StM_PlayerController player, CharacterControlsInput input) : base(player, input) { }

    public override void OnEnter()
    {
        _playerController.InitialJump = false;
        _playerController.CoyoteTimeCounter.Stop();
        _playerController.JumpTimer.Reset();
    }
    
    public override void Update()
    {
        OnJump();
    }


    public override void FixedUpdate()
    {
        _playerController.HandleRotation();
        HandleGravity();
        _playerController.PlayerMove();
    }
    
    private void HandleGravity()
    {
         var gravity = 0.0f;
        _playerController.GravityFallCurrent = _playerController.GravityFallMin;
        _playerController.PlayerFallTimer.Stop();
        _playerController.PlayerFallTimer.Reset(_playerController.PlayerFallTimeMax);
        _playerController.PlayerMoveInputY = gravity;
    }

    public override void OnExit()
    {
        _playerController.CoyoteTimeCounter.Start();
        
    }
    
    void OnJump()
    {
        if (_input.JumpIsPressed && !_playerController.JumpWasPressedLastFrame || _playerController.JumpBufferTimeCounter.IsRunning)
        {
            _playerController.JumpTimer.Start();
        }
        
        _playerController.JumpWasPressedLastFrame = _input.JumpIsPressed;
    }
}
