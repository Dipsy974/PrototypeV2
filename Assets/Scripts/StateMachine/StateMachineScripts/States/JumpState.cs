using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(StM_PlayerController player) : base(player) { }
    public override void OnEnter()
    {
        Debug.Log("entering jump");
        InitialJump();
    }

    public override void FixedUpdate()
    {
        HandleJump();
    }
    
    public override void OnExit()
    {
        Debug.Log("exiting jump");
    }
    
    private void InitialJump()
    {
        _playerController.PlayerMoveInputY = _playerController.InitialJumpForce;
        _playerController.JumpBufferTimeCounter.Stop();
        _playerController.CoyoteTimeCounter.Stop();
    }
    
    private void HandleJump()
    {
        
        var calculatedJumpInput = _playerController.InitialJumpForce * _playerController.ContinualJumpForceMultiplier;
        
        _playerController.PlayerMoveInputY =  calculatedJumpInput; 
    }
}
