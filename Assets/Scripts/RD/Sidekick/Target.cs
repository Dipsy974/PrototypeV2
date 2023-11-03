using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private TargetSystem _targetSystem;

    private void OnEnable()
    {
        _targetSystem = FindObjectOfType<TargetSystem>();
    }

    private void OnBecameVisible()
    {
        if (!_targetSystem.visibleTargets.Contains(this))
        {
            _targetSystem.visibleTargets.Add(this);
        }
    }

    private void OnBecameInvisible()
    {
        if (_targetSystem.visibleTargets.Contains(this))
        {
            _targetSystem.visibleTargets.Remove(this);
        }
    }
}
