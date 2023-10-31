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

    [Header("Jump Parameters")] 
    [SerializeField] float _initialJumpForce = 750.0f;
    [SerializeField] float _continualJumpForceMultiplier = 0.1f;
    [SerializeField] float _jumpTime = 0.175f;
    [SerializeField] float _coyoteTime = 0.15f;
    [SerializeField] float _jumpBufferTime = 0.2f;

    private Transform mainCam;

    private const float ZeroF = 0f;
    private float _currentSpeed, _velocity, _jumpVelocity;

    private Vector3 _movement;

    private List<Timer> _timers;
    private CountdownTimer _jumpTimer;
    private CountdownTimer _playerFallTimer , _coyoteTimeCounter, _jumpBufferTimeCounter;

    private StateMachine _stateMachine;
    
    //SIMPLE GETTERS
    public CountdownTimer JumpTimer { get { return _jumpTimer; } }
    public CountdownTimer PlayerFallTimer { get { return _playerFallTimer; } }
    public CountdownTimer CoyoteTimeCounter { get { return _coyoteTimeCounter; } }
    public CountdownTimer JumpBufferTimeCounter { get { return _jumpBufferTimeCounter; } }
    public float GravityFallMin { get { return _gravityFallMin; } }
    public float GravityFallMax { get { return _gravityFallMax; } }
    public float GravityFallIncrementAmount { get { return _gravityFallIncrementAmount; } }
    public float GravityFallIncrementTime { get { return _gravityFallIncrementTime; } }
    public float PlayerFallTimeMax { get { return _playerFallTimeMax; } }
    
    
    //GETTERS + SETTERS
    public float PlayerMoveInputY { get { return _playerMoveInput.y; } set { _playerMoveInput.y = value; } }
    public float InitialJumpForce { get { return _initialJumpForce; }set { _initialJumpForce = value; } }
    public float ContinualJumpForceMultiplier { get { return _continualJumpForceMultiplier; }set { _continualJumpForceMultiplier = value; } }
    public float GravityFallCurrent { get { return _gravityFallCurrent; }set { _gravityFallCurrent = value; } }


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

        _playerFallTimer = new CountdownTimer(_playerFallTimeMax);
        _coyoteTimeCounter = new CountdownTimer(_coyoteTime);
        _jumpBufferTimeCounter = new CountdownTimer(_jumpBufferTime);
        
        _timers = new List<Timer> { _jumpTimer, _playerFallTimer, _coyoteTimeCounter, _jumpBufferTimeCounter };
        
        //_jumpTimer.OnTimerStop += () => ;
        
        // State Machine creation
        _stateMachine = new StateMachine();
        
        // States creation
        var groundedState = new GroundedState(this, _input);
        var jumpState = new JumpState(this, _input);
        var fallState = new FallState(this, _input);
        
        // Transitions creation
        At(groundedState, jumpState, new FuncPredicate(()=> _jumpTimer.IsRunning));
        At(groundedState, fallState, new FuncPredicate(()=> !_jumpTimer.IsRunning && !_groundCheck.IsGrounded));
        
        At(jumpState, fallState, new FuncPredicate(()=> !_jumpTimer.IsRunning));
        
        At(fallState, groundedState, new FuncPredicate(()=> _groundCheck.IsGrounded));
        At(fallState, jumpState, new FuncPredicate(()=> _jumpTimer.IsRunning));
        
        
        // Set Initial State
        _stateMachine.SetState(groundedState);
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
        _playerMoveInput = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
 
        _stateMachine.FixedUpdate();

        _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
        
        _rigidbody.AddForce(_cameraRelativeMovement, ForceMode.Force);
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
    
    private Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = mainCam.forward;
        Vector3 cameraRight = mainCam.right;

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
    
    public void PlayerMove()
    {
        Vector3 calculatedPlayerMovement = (new Vector3(_playerMoveInput.x * _moveSpeed *_rigidbody.mass,
            _playerMoveInput.y * _rigidbody.mass,
            _playerMoveInput.z * _moveSpeed * _rigidbody.mass));
        
        _appliedMovement = calculatedPlayerMovement;
    }
    
    public void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;
        if (_input.Direction.magnitude > ZeroF && positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

    }
}