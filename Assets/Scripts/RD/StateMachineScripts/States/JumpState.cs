using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class JumpState : BaseState
{
    public JumpState(StM_PlayerController player, StM_InputReader input) : base(player, input) { }

    public override void OnEnter()
    {
        _input.Jump += OnJump;
        
        _playerController.CoyoteTimeCounter.Stop();
        InitialJump();
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
        
        _input.Jump -= OnJump;
    }
    
    private void InitialJump()
    {
        _playerController.PlayerMoveInputY = _playerController.InitialJumpForce;
    }
    
    private void HandleJump()
    {
        var calculatedJumpInput = _playerController.InitialJumpForce * _playerController.ContinualJumpForceMultiplier;
        _playerController.PlayerMoveInputY =  calculatedJumpInput; 
    }
    
    void OnJump(bool performed)
    {
        if (!performed)
        {
            _playerController.JumpTimer.Stop();
        }
    }
}
