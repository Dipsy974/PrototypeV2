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



}
