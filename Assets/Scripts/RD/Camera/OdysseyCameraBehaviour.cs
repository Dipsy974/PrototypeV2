using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OdysseyCameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private StM_PlayerController _playerController;
    [SerializeField] private float  _desiredSmoothTime;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _targetYOffset;

    private float _targetY;
    private Vector3 _vel;
    private Camera _mainCam;

    private void OnEnable()
    {
        _playerController.LeavingGround += OnLeavingGround;
    }

    void Start()
    {
        _mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnLeavingGround()
    {
        _targetY = _playerTransform.position.y;
    }
    
    void LateUpdate()
    {
        Vector3 viewPos = _mainCam.WorldToViewportPoint(_playerTransform.position + _playerController.Rigidbody.velocity * Time.deltaTime);
    
        // behavior 2
        if (viewPos.y > 0.95f || viewPos.y < 0.3f)
        {
            _targetY = _playerTransform.position.y;
        }
        if(_playerController.GroundCheck.IsGrounded)
        {
            _targetY = _playerTransform.position.y;
        }

        _targetTransform.rotation = _playerTransform.rotation;
        var desiredPosition = new Vector3(_playerTransform.position.x, _targetY + _targetYOffset, _playerTransform.position.z);
        _targetTransform.position = Vector3.SmoothDamp(_targetTransform.position, desiredPosition, ref _vel, _desiredSmoothTime, _followSpeed);
    }
}
