using UnityEngine.Events;

public class FallState : BaseState
{
    public FallState(StM_PlayerController player, StM_InputReader input) : base(player, input) {}
    

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
        _input.Jump -= OnJump;
    }
    
    private void HandleFallGravity()
    {
        if (_playerController.PlayerFallTimer.IsRunning)
        {
            _playerController.PlayerMoveInputY = 0f;
            return;
        }
        else if (_playerController.GravityFallCurrent > _playerController.GravityFallMax)
        {
            _playerController.GravityFallCurrent += _playerController.GravityFallIncrementAmount;
        }
        
        _playerController.PlayerFallTimer.Reset(_playerController.GravityFallIncrementTime);
        _playerController.PlayerMoveInputY = _playerController.GravityFallCurrent;
    }
    
    void OnJump(bool performed)
    {
        if (performed && _playerController.CoyoteTimeCounter.IsRunning)
        {
            _playerController.JumpTimer.Start();
        }
        else if (performed && !_playerController.JumpBufferTimeCounter.IsRunning)
        {
            _playerController.JumpBufferTimeCounter.Start();
        }
    }
}
