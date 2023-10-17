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

    [Header("Parameters")] 
    [SerializeField, Range(0.5f, 50f)] private float _speedMultiplier = 20f;

    private bool isRMBPressed;
    private bool cameraMovementLock;

    private void OnEnable()
    {
        _input.Look += OnLook;
        _input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        _input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }
    
    private void OnDisable()
    {
        _input.Look -= OnLook;
        _input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        _input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
    }
    private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
    {
        if (cameraMovementLock) return;
        if (isDeviceMouse && !isRMBPressed) return;

        float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        _freeLookCam.m_XAxis.m_InputAxisValue = cameraMovement.x * _speedMultiplier * deviceMultiplier;
        _freeLookCam.m_YAxis.m_InputAxisValue = cameraMovement.y * _speedMultiplier * deviceMultiplier;
    }
    
    private void OnEnableMouseControlCamera()
    {
        isRMBPressed = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DisableMouseForFrame());
    }
    
    private void OnDisableMouseControlCamera()
    {
        isRMBPressed = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _freeLookCam.m_XAxis.Value = 0f;
        _freeLookCam.m_YAxis.Value = 0f;
    }

    IEnumerator DisableMouseForFrame()
    {
        cameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        cameraMovementLock = false;
    }

}
