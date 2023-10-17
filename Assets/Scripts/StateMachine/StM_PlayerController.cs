using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class StM_PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private StM_GroundCheck _groundCheck;
    [SerializeField] private CinemachineFreeLook _freeLookCam;
    [SerializeField] private StM_InputReader _input;

    [Header("Movement Parameters")] 
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 15f;
    private Vector3 _playerMoveInput, _appliedMovement, _cameraRelativeMovement ;
    
    [Header("Gravity")]
    [SerializeField] private float _gravityFallCurrent = -100.0f;
    [SerializeField] private float _gravityFallMin = -100.0f;
    [SerializeField] private float _gravityFallMax = -500.0f;
    [SerializeField] [Range(-5f, -35f)] private float _gravityFallIncrementAmount = -20.0f;
    [SerializeField] private float _gravityFallIncrementTime = 0.05f;
    [SerializeField] private float _playerFallTimeMax = 0.3f;
    [SerializeField] private float _gravity = 0.0f;

    [Header("Jump Parameters")] 
    [SerializeField] float _initialJumpForce = 750.0f;
    [SerializeField] float _continualJumpForceMultiplier = 0.1f;
    [SerializeField] float _jumpTime = 0.175f;
    [SerializeField] float _coyoteTime = 0.15f;
    [SerializeField] float _jumpBufferTime = 0.2f;
    [SerializeField] bool _playerIsJumping = false;
    [SerializeField] bool _jumpWasPressedLastFrame = false;

    private Transform mainCam;

    private const float ZeroF = 0f;
    private float _currentSpeed, _velocity, _jumpVelocity;

    private Vector3 _movement;

    private List<Timer> _timers;
    private CountdownTimer _jumpTimer;
    private CountdownTimer _jumpCooldownTimer;
    private CountdownTimer _playerFallTimer , _coyoteTimeCounter, _jumpBufferTimeCounter;

    private StateMachine _stateMachine;

    private void Awake()
    {
        mainCam = Camera.main.transform;
        _freeLookCam.Follow = transform;
        _freeLookCam.LookAt = transform;
        
        //adjust position of camera if player is warped
        _freeLookCam.OnTargetObjectWarped(transform, transform.position - _freeLookCam.transform.position - Vector3.forward);

        _rigidbody.freezeRotation = true;
        
        //Timers setup
        _jumpTimer = new CountdownTimer(_jumpTime);
        _jumpCooldownTimer = new CountdownTimer(ZeroF);
        _playerFallTimer = new CountdownTimer(_playerFallTimeMax);
        _coyoteTimeCounter = new CountdownTimer(_coyoteTime);
        _jumpBufferTimeCounter = new CountdownTimer(_jumpBufferTime);
        
        _timers = new List<Timer> { _jumpTimer, _jumpCooldownTimer, _playerFallTimer };
        

        _jumpTimer.OnTimerStart += () => InitialJump();
        
        // State Machine
        _stateMachine = new StateMachine();
        
        // States
        var locomotionState = new LocomotionState(this);
        var jumpState = new JumpState(this);
        
        // Transitions
        At(locomotionState, jumpState, new FuncPredicate(()=> _jumpTimer.IsRunning));
        At(jumpState, locomotionState, new FuncPredicate(()=> _groundCheck.IsGrounded && !_jumpTimer.IsRunning));
        
        // Set Initial State
        _stateMachine.SetState(locomotionState);
    }

    void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    private void Start()
    {
        _input.EnablePlayerActions();
    }

    private void OnEnable()
    {
        _input.Jump += OnJump;
    }

    private void OnDisable()
    {
        _input.Jump += OnJump;
    }
    
    
    private void Update()
    {
        _stateMachine.Update();
        HandleTimers();
    }

    private void FixedUpdate()
    {
        HandleRotation();
        
        _playerMoveInput = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
        
        _playerMoveInput.y = PlayerGravity();
        _playerMoveInput.y = HandleJump();
        
        _appliedMovement = PlayerMove();
        _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
        
        _rigidbody.AddForce(_cameraRelativeMovement, ForceMode.Force);
        
        _stateMachine.FixedUpdate();
    }

    private void HandleTimers()
    {
        foreach (var timer in _timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }
    
    void OnJump(bool performed)
    {
        if (performed && !_jumpTimer.IsRunning && _groundCheck.IsGrounded)
        {
            _jumpTimer.Start();
        }
        else if (!performed && _jumpTimer.IsRunning)
        {
            _jumpTimer.Stop();
        }
        
    }
    private Vector3 PlayerMove()
    {
        Vector3 calculatedPlayerMovement = (new Vector3(_playerMoveInput.x * _moveSpeed *_rigidbody.mass,
            _playerMoveInput.y * _rigidbody.mass,
            _playerMoveInput.z * _moveSpeed * _rigidbody.mass));
        
        return calculatedPlayerMovement;
    }
    
    private float PlayerGravity()
    {
        if (_groundCheck.IsGrounded)
        {
            _gravity = 0.0f;
            _gravityFallCurrent = _gravityFallMin;
            _playerFallTimer.Pause();
            _playerFallTimer.Reset(_playerFallTimeMax);
        }
        else
        {
            _playerFallTimer.Resume();
            if (_playerFallTimer.IsFinished)
            {
                HandleFallGravity();
            }
        }

        return _gravity;
    }

    private float HandleFallGravity()
    {
        if (_gravityFallCurrent > _gravityFallMax)
        {
            _gravityFallCurrent += _gravityFallIncrementAmount;
        }
        _playerFallTimer.Reset(_gravityFallIncrementTime); 
        _gravity = _gravityFallCurrent;
        return _gravity;
    }
    
    private float HandleJump()
    {
        if (!_jumpTimer.IsRunning && _groundCheck.IsGrounded)
        {
            _coyoteTimeCounter.Stop();
            _coyoteTimeCounter.Reset();
            _jumpTimer.Stop();
            _jumpTimer.Reset();
            return ZeroF;
        }
        else
        {
            _coyoteTimeCounter.Resume();
        }
 
        float calculatedJumpInput = _playerMoveInput.y;
        _jumpBufferTimeCounter.Resume();
        
        if (_jumpTimer.IsRunning)
        {
            calculatedJumpInput = _initialJumpForce * _continualJumpForceMultiplier;
        }

        return calculatedJumpInput; 
    }

    private void InitialJump()
    {
        if(!_jumpBufferTimeCounter.IsFinished && !_playerIsJumping && !_coyoteTimeCounter.IsFinished)
        {
            _playerMoveInput.y = _initialJumpForce;
            _jumpBufferTimeCounter.Stop();
            _coyoteTimeCounter.Stop();
        }
    }
    
    public Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;
    }
    
    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;
        if (_input.Direction.magnitude > ZeroF)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

    }
}
