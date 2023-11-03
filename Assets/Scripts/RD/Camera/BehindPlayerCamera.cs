using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BehindPlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private StM_PlayerController _playerController;
    [SerializeField] private CharacterControlsInput _input;
    [SerializeField] private float _forwardInputRecenteringTime;
    [SerializeField] private float _backwardsInputRecenteringTime;
    [SerializeField] private float _forwardInputThreshold = -0.1f;
    
    
    private CinemachineFreeLook _camera;
    private Camera _mainCam;
    private bool _hasStoppedRecentering;
    
    private void OnEnable()
    {
        _camera = GetComponentInChildren<CinemachineFreeLook>();
    }
    
    private void Update()
    {
        if (_input.MoveInput.y >= _forwardInputThreshold)
        {
            _camera.m_RecenterToTargetHeading.m_RecenteringTime = _forwardInputRecenteringTime;
        }
        else
        {
            _camera.m_RecenterToTargetHeading.m_RecenteringTime = _backwardsInputRecenteringTime;
        }
    }
}
