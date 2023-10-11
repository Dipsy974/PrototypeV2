using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class StM_PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private CharacterController _controller;
    //[SerializeField] private Animator _animator;
    [SerializeField] private CinemachineFreeLook _freeLookCam;
    [SerializeField] private StM_InputReader _input;

    [Header("Parameters")] 
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _smoothTime = 0.1f;

    private Transform mainCam;

    private const float ZeroF = 0f;
    private float _currentSpeed, _velocity;

    private void Awake()
    {
        mainCam = Camera.main.transform;
        _freeLookCam.Follow = transform;
        _freeLookCam.LookAt = transform;
        
        //adjust position of camera if player is warped
        _freeLookCam.OnTargetObjectWarped(transform, transform.position - _freeLookCam.transform.position - Vector3.forward);
        
    }

    private void Start()
    {
        _input.EnablePlayerActions();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        var movementDirection = new Vector3(_input.Direction.x,0f, _input.Direction.y).normalized;
        
        //Make movement match camera rotation
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirection;
        if (adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);
            HandleCharacterController(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(ZeroF);
        }
    }

    private void HandleCharacterController(Vector3 adjustedDirection)
    {
        //move
        var adjustedMovement = adjustedDirection * (_moveSpeed * Time.deltaTime);
        _controller.Move(adjustedMovement);
    }

    private void HandleRotation(Vector3 adjustedDirection)
    {
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustedDirection);
    }

    private void SmoothSpeed(float value)
    {
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
    }
}
