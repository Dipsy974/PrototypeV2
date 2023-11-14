using UnityEngine;
using UnityEngine.Events;

public class FallState : BaseState
{
    public FallState(StM_PlayerController player, StM_InputReader input) : base(player, input)
    {
        
    }


    public override void OnEnter()
    {
        _input.Jump += OnJump;
    }
    
    public override void FixedUpdate()
    {
        _playerController.HandleRotation();
        HandleFallGravity();
        _playerController.PlayerMove();
    }

    public override void OnExit()
    {
        if (!_playerController.InitialJump)
        {
            _playerController.InitialJump = true;
        }
        _input.Jump -= OnJump;
    }
    
    private void HandleFallGravity()
    {
        if (_playerController.PlayerFallTimer.IsRunning)
        {
            _playerController.PlayerMoveInputY = 0f;
            return;
        }
        
        if (_playerController.GravityFallCurrent > _playerController.GravityFallMax)
        {
            _playerController.GravityFallCurrent += _playerController.GravityFallIncrementAmount;
        }
        
        _playerController.PlayerFallTimer.Reset(_playerController.GravityFallIncrementTime);
        _playerController.PlayerMoveInputY = _playerController.GravityFallCurrent;
    }
    
    void OnJump(bool jumpisPressed)
    {
        if (jumpisPressed && _playerController.CoyoteTimeCounter.IsRunning)
        { 
            _playerController.JumpTimer.Start();
        }
        if (jumpisPressed && !_playerController.JumpBufferTimeCounter.IsRunning && !_playerController.JumpWasPressedLastFrame)
        {
            _playerController.JumpBufferTimeCounter.Start();
        }
        _playerController.JumpWasPressedLastFrame = jumpisPressed;
    }
}
