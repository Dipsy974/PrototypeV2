using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : MonoBehaviour
{
    [SerializeField] private TargetSystem _targetSystem;
    [SerializeField] private ParticleSystem _correctTrajectoryEmission;
    [SerializeField] private Transform _sidekickThrowOrigin;
    [SerializeField] private CharacterControlsInput _input;

    private Target _lockedTarget;

    private void Update()
    {
        if (_input.ThrowIsPressed && _targetSystem.currentTarget != null)
        {
            ThrowSidekick();
        }
    }

    void ThrowSidekick()
    {
        _lockedTarget = _targetSystem.currentTarget;
        _correctTrajectoryEmission.transform.position = _lockedTarget.transform.position;
        var shape = _correctTrajectoryEmission.shape;
        shape.position = _correctTrajectoryEmission.transform.InverseTransformPoint(_sidekickThrowOrigin.position);
        _correctTrajectoryEmission.Play();
    }
}
