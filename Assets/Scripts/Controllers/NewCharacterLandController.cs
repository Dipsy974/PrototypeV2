using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class NewCharacterLandController : MonoBehaviour
{
    [SerializeField] private CharacterControlsInput _input;
    [SerializeField] private CinemachineInputProvider _cinemachineInput;
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;
    private Animator _animator;
    public CameraController _camController;

    public ParticleSystem inkParticle; 


    //Movement variables
    private Vector3 _playerMoveInput;
    private Vector3 _appliedMovement;
    private Vector3 _cameraRelativeMovement;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _crouchSpeedMultiplier = 0.5f;
    private float _speedMultiplier;
    private float _desiredMoveSpeed, _lastDesiredMoveSpeed;
    

    private bool _isRolling = false; 
    private bool _isAttacking = false; 
    private bool _isHanging = false;

    private bool _canBounce = false; 
    private bool _isBouncing = false; 


    [Header("Ground Check")]
    [SerializeField] private bool _playerIsGrounded = true;
    [SerializeField] [Range(0.0f, 1.8f)] private float _groundCheckRadiusMultiplier = 0.9f;
    [SerializeField] [Range(-0.95f, 1.05f)] private float _groundCheckDistance = 0.05f;
    private RaycastHit _groundCheckHit = new RaycastHit();

    [Header("Gravity")]
    [SerializeField] private float _gravityFallCurrent = -100.0f;
    [SerializeField] private float _gravityFallMin = -100.0f;
    [SerializeField] private float _gravityFallMax = -500.0f;
    [SerializeField] [Range(-5f, -35f)] private float _gravityFallIncrementAmount = -20.0f;
    [SerializeField] private float _gravityFallIncrementTime = 0.05f;
    [SerializeField] private float _playerFallTimer = 0.3f;
    [SerializeField] private float _playerFallTimeMax = 0.3f;
    [SerializeField] private float _gravity = 0.0f;

    [Header("Jump")]
    [SerializeField] float _initialJumpForce = 750.0f;
    [SerializeField] float _continualJumpForceMultiplier = 0.1f;
    [SerializeField] float _jumpTime = 0.175f;
    [SerializeField] float _jumpTimeCounter = 0.0f;
    [SerializeField] float _coyoteTime = 0.15f;
    [SerializeField] float _coyoteTimeCounter = 0.0f;
    [SerializeField] float _jumpBufferTime = 0.2f;
    [SerializeField] float _jumpBufferTimeCounter = 0.0f;
    [SerializeField] bool _playerIsJumping = false;
    [SerializeField] bool _jumpWasPressedLastFrame = false;
    private bool _isFalling, _isCrouching;

    [Header("Bounce")]
    [SerializeField] float _bounceForce = 10.0f;
    [SerializeField] float _bounceTime = 0.3f;
    private Vector3 _bounceDirection; 


    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isFallingHash;
    private int _isCrouchingHash;
   

    private float _rotationFactorPerFrame = 15.0f;

    //GETTERS AND SETTERS
    public Rigidbody RB { get { return _rb; } }
    public CapsuleCollider CapCollider { get { return _capsuleCollider; } }
    public Animator Animator { get { return _animator; } }
    public Vector3 PlayerMoveInput { get { return _playerMoveInput; } private set { } }
    public bool PlayerIsGrounded { get { return _playerIsGrounded; } private set { } }
    public bool IsRolling { get { return _isRolling; }  set { _isRolling = value; } }
    public bool IsAttacking { get { return _isAttacking; }  set { _isAttacking = value; } }
    public bool IsHanging { get { return _isHanging; }  set { _isHanging = value; } }
    public bool IsFalling { get { return _isFalling; }  set { _isFalling = value; }}
    public bool IsJumping { get { return _playerIsJumping; }  set { _playerIsJumping = value; }}
    public bool IsCrouching { get { return _isCrouching; }  set { _isCrouching = value; }}
    public bool CanBounce { get { return _canBounce; }  set { _canBounce = value; }}
    public bool IsBouncing { get { return _isBouncing; }  set { _isBouncing = value; }}
    public float BounceForce { get { return _bounceForce; }}
    public float BounceTime { get { return _bounceTime; }}





    //public float runMultiplyer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();


        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isFallingHash = Animator.StringToHash("isFalling");
        _isCrouchingHash = Animator.StringToHash("isCrouching");

        //LOGIQUE POUR CACHER LE CURSEUR, A PLACER AILLEURS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _cinemachineInput.XYAxis.Set(_input.PlayerInput.CharacterControls.Look);

        _speedMultiplier = 1f;

    }


    void FixedUpdate()
    {
        if (CheckFocusCamera())
        {
            PlayerLook();
            _isCrouching = true;
            _speedMultiplier = _crouchSpeedMultiplier;
        }
        else
        {
            _isCrouching = false;
            _speedMultiplier = 1f;
            if (!_isRolling && !_isHanging)
            {
                HandleRotation();
            }
        }

   
        //Check isFalling
        _isFalling = _rb.velocity.y < 0 && !_playerIsGrounded;
        
        HandleAnimation();

        //MOVEMENT
        _playerMoveInput = GetMoveInput(); //Get Data from InputSystem
        _playerIsGrounded = PlayerGroundCheck();

        if (!_isBouncing)
        {
            _playerMoveInput.y = PlayerGravity();
            _playerMoveInput.y = PlayerJump();
        }
        else
        {
            PlayerBounce(_bounceDirection);
        }
        

        _appliedMovement = PlayerMove();
        _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);

        if (!_isRolling && !_isAttacking && !_isHanging)
        {
            _rb.AddForce(_cameraRelativeMovement, ForceMode.Force);
        }
 

    }



    private Vector3 PlayerMove()
    {
        Vector3 calculatedPlayerMovement = (new Vector3(_playerMoveInput.x * _movementSpeed *_rb.mass * _speedMultiplier,
                                        _playerMoveInput.y * _rb.mass,
                                        _playerMoveInput.z * _movementSpeed * _rb.mass * _speedMultiplier));

       

        return calculatedPlayerMovement;
    }

    private void PlayerLook()
    {
        _rb.rotation = Quaternion.Euler(0.0f, Camera.main.transform.rotation.eulerAngles.y, 0.0f);
    }

    private void HandleAnimation()
    {
        bool isRunningAnimating = _animator.GetBool(_isRunningHash);

        //Walking handling
        if (_input.MoveIsPressed)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        else
        {
            _animator.SetBool(_isRunningHash, false);
        }

        //Jumping handling
        if (_playerIsJumping)
        {
            _animator.SetBool(_isJumpingHash, true);
        }
        else if (!_playerIsJumping || _playerIsGrounded)
        {
            _animator.SetBool(_isJumpingHash, false);
        }
        
        if (!_playerIsJumping && !_playerIsGrounded || _isFalling)
        {
            _animator.SetBool(_isFallingHash, true);
        }
        else if (!_isFalling)
        {
            _animator.SetBool(_isFallingHash, false);
        }
        
        //crouch

        if (_isCrouching)
        {
            _animator.SetBool(_isCrouchingHash, true);
        }
        else
        {
            _animator.SetBool(_isCrouchingHash, false);
        }

    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;
        if (_input.MoveIsPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }

    }


    private bool PlayerGroundCheck()
    {
        float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
        float sphereCastTravelDistance = _capsuleCollider.bounds.extents.y - sphereCastRadius + _groundCheckDistance;
        return Physics.SphereCast(_rb.position, sphereCastRadius, Vector3.down, out _groundCheckHit, sphereCastTravelDistance, LayerMask.GetMask("Walls"));
    }

    private float PlayerGravity()
    {
        if (_playerIsGrounded || _isHanging)
        {
            _gravity = 0.0f;
            _gravityFallCurrent = _gravityFallMin;
            _playerFallTimer = _playerFallTimeMax;
        }
        else
        { 
            _playerFallTimer -= Time.fixedDeltaTime;
            if (_playerFallTimer < 0.0f)
            {
                if (_gravityFallCurrent > _gravityFallMax)
                {
                    _gravityFallCurrent += _gravityFallIncrementAmount;
                }
                _playerFallTimer = _gravityFallIncrementTime;
                _gravity = _gravityFallCurrent;
            }
        }

        return _gravity;
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

    public Vector3 GetMoveInput()
    {
        return new Vector3(_input.MoveInput.x, 0.0f, _input.MoveInput.y);
    }

    private float PlayerJump()
    {
 
        float calculatedJumpInput = _playerMoveInput.y;

        SetJumpTimeCounter();
        SetCoyoteTimeCounter();
        SetJumpBufferTimeCounter();

        if(_jumpBufferTimeCounter > 0.0f && !_playerIsJumping && _coyoteTimeCounter > 0.0f && !_isCrouching)
        {
            calculatedJumpInput = _initialJumpForce;
            _playerIsJumping = true;
            _jumpBufferTimeCounter = 0.0f;
            _coyoteTimeCounter = 0.0f;

            inkParticle.Play();
            Debug.Log("je saute"); 
            
        }
        else if (_input.JumpIsPressed && _playerIsJumping && !_playerIsGrounded && _jumpTimeCounter > 0.0f)
        {
            calculatedJumpInput = _initialJumpForce * _continualJumpForceMultiplier;
        }
        else if(_playerIsJumping && _playerIsGrounded)
        {
            _playerIsJumping = false;
        }

        return calculatedJumpInput; 
    }

    public void PlayerBounce(Vector3 direction)
    {
        _rb.AddForce(direction * _bounceForce, ForceMode.Impulse); 
        //_rb.velocity = direction * _bounceForce; 
    }

    public void SetBounceDirection(Vector3 direction)
    {
         _bounceDirection = direction; 
    }

    private void SetJumpTimeCounter()
    {
        if(_playerIsJumping && !_playerIsGrounded)
        {
            _jumpTimeCounter -= Time.fixedDeltaTime;
        }
        else
        {
            _jumpTimeCounter = _jumpTime; 
        }
    }

    private void SetCoyoteTimeCounter()
    {
        if (_playerIsGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void SetJumpBufferTimeCounter()
    {
        if(!_jumpWasPressedLastFrame && _input.JumpIsPressed)
        {
            _jumpBufferTimeCounter = _jumpBufferTime;

        }else if(_jumpBufferTimeCounter > 0.0f)
        {
            _jumpBufferTimeCounter -= Time.fixedDeltaTime;
        }
        _jumpWasPressedLastFrame = _input.JumpIsPressed; 
    }

    private bool CheckFocusCamera()
    {
        return _camController.ActiveCamera == _camController._focusCamera;  //&& !_camController.IsLiveBlend; 
    }

    public IEnumerator CancelBounce()
    {
        yield return new WaitForSeconds(_bounceTime);
        _isBouncing = false;
    }


}
