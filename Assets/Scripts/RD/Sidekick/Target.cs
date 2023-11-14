using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private TargetSystem _targetSystem;
    [SerializeField] private Transform _playerTransform;
    public bool isReachable;

    private Camera _mainCam;
    private void OnEnable()
    {
        _targetSystem = FindObjectOfType<TargetSystem>();
        _playerTransform = FindObjectOfType<StM_PlayerController>().transform;
    }

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        Vector3 playerToObject = transform.position - _playerTransform.position;
        bool isBehindPlayer = Vector3.Dot(_mainCam.transform.forward, playerToObject) < 0;

        if (Vector3.Distance(transform.position, _playerTransform.position) < _targetSystem.minReachDistance && !isBehindPlayer && !isReachable)
        {
            isReachable = true;
            if (_targetSystem.visibleTargets.Contains(this))
            {
                _targetSystem.reachableTargets.Add(this);
            }
        }

        if ((Vector3.Distance(transform.position, _playerTransform.position) > _targetSystem.minReachDistance || isBehindPlayer) && isReachable)
        {
            isReachable = false;
            if (_targetSystem.reachableTargets.Contains(this))
            {
                _targetSystem.reachableTargets.Remove(this);
            }
        }
    }

    private void OnBecameVisible()
    {
        if (!_targetSystem.visibleTargets.Contains(this))
        {
            _targetSystem.visibleTargets.Add(this);
            
            if(isReachable)
                _targetSystem.reachableTargets.Add(this);
        }
    }

    private void OnBecameInvisible()
    {
        if (_targetSystem.visibleTargets.Contains(this))
        {
            _targetSystem.visibleTargets.Remove(this);
            
            if (_targetSystem.reachableTargets.Contains(this))
            {
                _targetSystem.reachableTargets.Remove(this);
            }
        }
    }
}
