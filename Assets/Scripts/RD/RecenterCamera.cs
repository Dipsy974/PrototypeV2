using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RecenterCamera : MonoBehaviour
{
    [SerializeField] private CharacterControlsInput _input;
    private CinemachineFreeLook _camera;
    private bool _isRecentering;
    private void OnEnable()
    {
        _camera = GetComponentInChildren<CinemachineFreeLook>();
    }

    private void Update()
    {
        if (_input.CameraChangeIsPressed && !_isRecentering)
        {
            StartCoroutine(Recenter());
        }
    }

    private IEnumerator Recenter()
    {
        _isRecentering = true;
        _camera.m_RecenterToTargetHeading.m_enabled = true;
        yield return new WaitForSeconds(_camera.m_RecenterToTargetHeading.m_RecenteringTime * 2f);
        _camera.m_RecenterToTargetHeading.m_enabled = false;
        _isRecentering = false;
    }
}
