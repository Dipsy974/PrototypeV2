using UnityEngine;
using UnityEngine.Events;

public class FallState : BaseState
{
    public FallState(StM_PlayerController player, CharacterControlsInput input) : base(player, input) {}


    public override void OnEnter()
    {

    }
    
    public override void Update()
    {
        OnJump();
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
    
    void OnJump()
    {
        if (_input.JumpIsPressed && _playerController.CoyoteTimeCounter.IsRunning)
        {
            _playerController.JumpTimer.Start();
        }
        if (_input.JumpIsPressed && !_playerController.JumpBufferTimeCounter.IsRunning && !_playerController.JumpWasPressedLastFrame)
        {
            _playerController.JumpBufferTimeCounter.Start();
        }
        _playerController.JumpWasPressedLastFrame = _input.JumpIsPressed;
    }
}
