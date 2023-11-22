using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : MonoBehaviour
{
    [SerializeField] private TargetSystem _targetSystem;
    [SerializeField] private ParticleSystem _correctTrajectoryEmission;
    [SerializeField] private ParticleSystem _retrieveTrajectoryEmission;
    [SerializeField] private Transform _sidekickThrowOrigin;
    [SerializeField] private CharacterControlsInput _input;
    
    private Target _lockedTarget, _lastActivatedTarget;
    private bool _sidekickIsAvailable = true, _throwWasPressedLastFrame, _isRetrieving;

    private void Update()
    {
        if (_input.ThrowIsPressed && _sidekickIsAvailable && _targetSystem.currentTarget != null && !_throwWasPressedLastFrame)
        {
            ThrowSidekick();
        }
        else if (_input.ThrowIsPressed && !_throwWasPressedLastFrame && !_isRetrieving)
        {
            RetrieveSidekick();
        }
    }

    private void LateUpdate()
    {
        _throwWasPressedLastFrame = _input.ThrowIsPressed;
    }

    private void ThrowSidekick()
    {
        _lockedTarget = _targetSystem.currentTarget;
        _lastActivatedTarget = _lockedTarget;
        _correctTrajectoryEmission.transform.position = _lockedTarget.transform.position;
        var shape = _correctTrajectoryEmission.shape;
        shape.position = _correctTrajectoryEmission.transform.InverseTransformPoint(_sidekickThrowOrigin.position);
        _correctTrajectoryEmission.Play();
        _sidekickIsAvailable = false;
    }

    private void RetrieveSidekick()
    {
        if (_lastActivatedTarget != null)
        {
            _isRetrieving = true;
            StartCoroutine(UpdateSidekickRetrievePoint());
            var shape = _retrieveTrajectoryEmission.shape;
            shape.position = _retrieveTrajectoryEmission.transform.InverseTransformPoint(_lastActivatedTarget.transform.position);
            _retrieveTrajectoryEmission.Play();
        }
    }

    public void StopRetrieve()
    {
        _isRetrieving = false;
        _sidekickIsAvailable = true;
    }

    private IEnumerator UpdateSidekickRetrievePoint()
    {
        while (_isRetrieving)
        {
            _retrieveTrajectoryEmission.transform.position = _sidekickThrowOrigin.position;
            yield return new WaitForEndOfFrame();
        }
        
    }
}
