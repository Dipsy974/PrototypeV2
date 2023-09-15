using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLandController : MonoBehaviour
{
    public Transform cameraFollow;
    public Transform focusFollow;

    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;
    private Animator _animator;

    private int _isRunningHash; 

    [SerializeField] private CharacterControlsInput _input;

    private Vector3 _playerMoveInput;

    private Vector3 _playerLookInput; 
    private Vector3 _previousPlayerLookInput; 
    private float _cameraPitch = 0.0f;
    [SerializeField] private float _playerLookInputLerpTime = 0.35f;

    [Header("Movement")]
    [SerializeField] private float _movementMultiplier = 30.0f;
    [SerializeField] private float _rotationSpeedMultiplier = 180.0f;
    [SerializeField] private float _pitchSpeedMultiplier = 180.0f;

    [Header("Ground Check")]
    [SerializeField] private bool _playerIsGrounded = true; 
    [SerializeField] [Range(0.0f,1.8f)] private float _groundCheckRadiusMultiplier = 0.9f; 
    [SerializeField] [Range(-0.95f, 1.05f)] private float _groundCheckDistance = 0.05f;
    private RaycastHit _groundCheckHit = new RaycastHit();

    [Header("Gravity")]
    [SerializeField] private float _gravityFallCurrent = -100.0f;
    [SerializeField] private float _gravityFallMin = -100.0f;
    [SerializeField] private float _gravityFallMax = -500.0f;
    [SerializeField] [Range(-5f, -35f)] private float _gravityFallIncrementAmount = -20.0f;
    [SerializeField] private float _gravityFallIncrementTime = 0.05f;
    [SerializeField] private float _playerFallTimer = 0.0f;
    [SerializeField] private float _gravity = 0.0f; 

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>(); 

        _isRunningHash = Animator.StringToHash("isRunning");
        //LOGIQUE POUR CACHER LE CURSEUR, A PLACER AILLEURS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        //LOOK
        _playerLookInput = GetLookInput();
        PlayerLook();
        PitchCamera(); 

        //MOVEMENT
        _playerMoveInput = GetMoveInput(); //Get Data from InputSystem
        _playerIsGrounded = PlayerGroundCheck();
        _playerMoveInput.y = PlayerGravity();

        //ANIMATION
        HandleAnimation(); 
        
        _playerMoveInput = PlayerMove(); //Calculate Vector3 to apply according _movementMultiplier and Rigidbody mass

        //Actually move the Player
        _rb.AddRelativeForce(_playerMoveInput, ForceMode.Force); 
    }

    private Vector3 GetMoveInput()
    {
        return new Vector3(_input.MoveInput.x, 0.0f, _input.MoveInput.y);  
    }


    private bool PlayerGroundCheck()
    {
        float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
        float sphereCastTravelDistance = _capsuleCollider.bounds.extents.y - sphereCastRadius + _groundCheckDistance;
        return Physics.SphereCast(_rb.position, sphereCastRadius, Vector3.down, out _groundCheckHit, sphereCastTravelDistance);
    }

    private float PlayerGravity()
    {
        if (_playerIsGrounded)
        {
            _gravity = 0.0f;
            _gravityFallCurrent = _gravityFallMin;
        }
        else
        {
            _playerFallTimer -= Time.fixedDeltaTime;
            if(_playerFallTimer < 0.0f)
            {
                if(_gravityFallCurrent > _gravityFallMax)
                {
                    _gravityFallCurrent += _gravityFallIncrementAmount;
                }
                _playerFallTimer = _gravityFallIncrementTime;
                _gravity = _gravityFallCurrent; 
            }
        }

        return _gravity; 
    }

    private Vector3 GetLookInput()
    {
        _previousPlayerLookInput = _playerLookInput;
        _playerLookInput = new Vector3(_input.LookInput.x, (_input.InvertMouseY ? -_input.LookInput.y : _input.LookInput.y), 0.0f);
        return Vector3.Lerp(_previousPlayerLookInput, _playerLookInput * Time.deltaTime, _playerLookInputLerpTime); 
    }

    private void PlayerLook()
    {
        _rb.rotation = Quaternion.Euler(0.0f, _rb.rotation.eulerAngles.y + (_playerLookInput.x * _rotationSpeedMultiplier), 0.0f); 
    }

    private void PitchCamera()
    {
        Vector3 rotationValues = cameraFollow.rotation.eulerAngles;
        _cameraPitch += _playerLookInput.y * _pitchSpeedMultiplier;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -89.9f, 89.9f);

        cameraFollow.rotation = Quaternion.Euler(_cameraPitch, rotationValues.y, rotationValues.z);
        focusFollow.rotation = cameraFollow.rotation; 
    }

    private Vector3 PlayerMove()
    {
        Vector3 calculatedPlayerMovement = (new Vector3(_playerMoveInput.x * _movementMultiplier * _rb.mass,
                                        _playerMoveInput.y * _rb.mass,
                                        _playerMoveInput.z * _movementMultiplier * _rb.mass));

        return calculatedPlayerMovement; 
    }

    private void HandleAnimation()
    {
        bool isRunning = _animator.GetBool(_isRunningHash);

        //Running handling
        if (_playerMoveInput != Vector3.zero)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        else
        {
            _animator.SetBool(_isRunningHash, false);
        }

    }

}
