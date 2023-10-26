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
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _followSpeed;
    

    [Header("Parameters")] 
    [SerializeField, Range(0.5f, 50f)] private float _speedMultiplier = 20f;

    private float _cameraTargetY;
    private Camera _mainCam;
    private Vector3 _velocityRef;
    private void Awake()
    {
        _mainCam = Camera.main;
        _playerController.LeavingGround += OnLeavingGround;
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
        _cameraTargetY = _playerController.transform.position.y;
    }

    private void LateUpdate()
    {
        Vector3 viewPos = _mainCam.WorldToViewportPoint(_playerController.transform.position + _playerController.Rigidbody.velocity * Time.deltaTime);
        
        if (viewPos.y > 0.85f || viewPos.y < 0.1f)
        {
            _cameraTargetY = _playerController.transform.position.y;
        }
        else if(_playerController.GroundCheck.IsGrounded)
        {
            _cameraTargetY = _playerController.transform.position.y;
        }

        _cameraTargetTransform.position = new Vector3(_playerController.transform.position.x, _cameraTargetY,
            _playerController.transform.position.z);
        
    }

    private IEnumerator SmoothMovement(Vector3 targetPos)
    {
        var basePos = _cameraTargetTransform.position;
        var interpolation = 0f;
        while (interpolation < 1f)
        {
            _cameraTargetTransform.position = Vector3.Lerp(basePos, targetPos, interpolation);
            interpolation += Time.deltaTime * _smoothTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
