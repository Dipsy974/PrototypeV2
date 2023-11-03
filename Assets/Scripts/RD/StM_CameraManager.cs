using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class StM_CameraManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private StM_InputReader _input;
    [SerializeField] private CinemachineFreeLook _freeLookCam;
    [SerializeField] private StM_PlayerController _playerController;
    [SerializeField] private Transform _cameraTargetTransform;
    [SerializeField] private Transform _cameraTargetParentTransform;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _followSpeed;
    

    [Header("Parameters")] 
    [SerializeField, Range(0.5f, 50f)] private float _speedMultiplier = 20f;

    private float _cameraTargetY;
    private Camera _mainCam;
    private Vector3 _velocityRef;
    private bool _following;
    
    private void Awake()
    {
        _following = true;
        _mainCam = Camera.main;
        _playerController.LeavingGround += OnLeavingGround;
        _playerController.EnteringGround += OnEnteringGround;
    }

    private void OnEnable()
    {
        _input.Look += OnLook;
    }
    
    private void OnDisable()
    {
        _input.Look -= OnLook;
    }
    private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
    {

        float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        _freeLookCam.m_XAxis.m_InputAxisValue = cameraMovement.x * _speedMultiplier * deviceMultiplier;
        _freeLookCam.m_YAxis.m_InputAxisValue = cameraMovement.y * _speedMultiplier * deviceMultiplier;
    }

    private void Start()
    {
        EnableControlCamera();
    }

    public void EnableControlCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void DisableControlCamera()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _freeLookCam.m_XAxis.Value = 0f;
        _freeLookCam.m_YAxis.Value = 0f;
    }

    public void OnLeavingGround()
    {
        _following = false;
    }
    public void OnEnteringGround()
    {
        var targetY = _playerController.transform.position.y;
        StartCoroutine(SmoothYMovement(targetY, _smoothTime));
        _following = true;
    }
    

    private void LateUpdate()
    {
        
        // Vector3 viewPos = _mainCam.WorldToViewportPoint(_playerController.transform.position + _playerController.Rigidbody.velocity * Time.deltaTime);
        // if (viewPos.y > 0.98f )
        // {
        //     _following = true;
        // }
        
        
        if (_following)
        {
            if(_cameraTargetTransform.position != _playerController.transform.position) _cameraTargetTransform.position = _playerController.transform.position;
            if (_cameraTargetTransform.parent != _playerController.transform)
            {
                _cameraTargetTransform.parent = _playerController.transform;
                _cameraTargetTransform.rotation = _playerController.transform.rotation;
            }
        }
        else
        {
            if (_cameraTargetTransform.parent != _cameraTargetParentTransform.transform)_cameraTargetTransform.parent = _cameraTargetParentTransform.transform;
            _cameraTargetTransform.position = new Vector3(_playerController.transform.position.x, _cameraTargetTransform.position.y,_playerController.transform.position.z);
        }
        
    }

    private IEnumerator SmoothYMovement(float targetY,float smoothSpeed)
    {
        var basePos = _cameraTargetTransform.position;
        var interpolation = 0f;
        while (interpolation < 0.95f)
        {
            _cameraTargetTransform.position = new Vector3(_playerController.transform.position.x, Mathf.Lerp(basePos.y, targetY, interpolation), _playerController.transform.position.z);
            interpolation += Time.deltaTime * smoothSpeed;
            yield return new WaitForEndOfFrame();
        }
    }
}
