using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    [SerializeField] 
    private CharacterControlsInput _input;

    private Rigidbody _rigidbody;
    private Transform _orientation, _playerTransform;

    [Header("Sliding")] 
    [SerializeField] private float _maxSlideTime;
    [SerializeField] private float _slideForce;

    private float _slideTimer;
    private bool _sliding, _slideJustPressed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = _characterController.RB;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_sliding && _input.MoveIsPressed && _input.SlideIsPressed && !_slideJustPressed)
        {
            _slideJustPressed = true;
            StartSlide();
        }
        else if (_sliding && !_input.SlideIsPressed)
        {
            StopSlide();
        }
        else if (!_input.SlideIsPressed)
        {
            _slideJustPressed = false;
        }
    }

    private void FixedUpdate()
    {
        if (_sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        _sliding = true;
        _slideTimer = _maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 direction = _characterController.ConvertToCameraSpace(_characterController.GetMoveInput());
        
        _rigidbody.AddForce(direction.normalized * _slideForce, ForceMode.Force);
        _slideTimer -= Time.deltaTime;
        
        Debug.Log("sliding");
        
        if(_slideTimer <= 0) StopSlide();
    }

    private void StopSlide()
    {
        _sliding = false;
    }
}
