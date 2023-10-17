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
    [SerializeField] private float _smoothTime = 0.1f;
    
    [Header("Jump Parameters")] 
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _jumpDuration = 0.5f;
    [SerializeField] private float _jumpCooldown = 0f;
    [SerializeField] private float _jumpMaxHeight = 2f;
    [SerializeField] private float _gravityMultiplier = 3f;

    private Transform mainCam;

    private const float ZeroF = 0f;
    private float _currentSpeed, _velocity, _jumpVelocity;

    private Vector3 _movement;

    private List<Timer> _timers;
    private CountdownTimer _jumpTimer;
    private CountdownTimer _jumpCooldownTimer;

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
        _jumpTimer = new CountdownTimer(_jumpDuration);
        _jumpCooldownTimer = new CountdownTimer(_jumpCooldown);
        _timers = new List<Timer> { _jumpTimer, _jumpCooldownTimer };

        _jumpTimer.OnTimerStart += () => _jumpVelocity = _jumpForce;
        _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();
        
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

    void OnJump(bool performed)
    {
        if (performed && !_jumpTimer.IsRunning && !_jumpCooldownTimer.IsRunning && _groundCheck.IsGrounded)
        {
            _jumpTimer.Start();
        }
        else if (!performed && _jumpTimer.IsRunning)
        {
            _jumpTimer.Stop();
        }
        
    }
    
    private void Update()
    {
        _movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
        _stateMachine.Update();

        HandleTimers();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    private void HandleTimers()
    {
        foreach (var timer in _timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    public void HandleJump()
    {
        if (!_jumpTimer.IsRunning && _groundCheck.IsGrounded)
        {
            _jumpVelocity = ZeroF;
            _jumpTimer.Stop();
            return;
        }

        if (!_jumpTimer.IsRunning){
            _jumpVelocity += Physics.gravity.y * _gravityMultiplier * Time.fixedDeltaTime;
        }
        
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpVelocity, _rigidbody.velocity.z);
    }

    public void HandleMovement()
    {
        
        //Make movement match camera rotation
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * _movement;
        if (adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);
            HandleHorizontalMovement(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(ZeroF);
            _rigidbody.velocity = new Vector3(ZeroF, _rigidbody.velocity.y, ZeroF);
        }
    }

    private void HandleHorizontalMovement(Vector3 adjustedDirection)
    {
        //move
        Vector3 velocity = adjustedDirection * (_moveSpeed * Time.fixedDeltaTime);
        _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);
        
    }

    private void HandleRotation(Vector3 adjustedDirection)
    {
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void SmoothSpeed(float value)
    {
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
    }
}
