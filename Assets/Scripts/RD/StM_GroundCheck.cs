using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StM_GroundCheck : MonoBehaviour
{
    [SerializeField] private float _groundDistance = 0.08f;
    [SerializeField] private LayerMask _groundLayers;

    public event UnityAction LeavingGround = delegate {  };
    
    private bool previousState;
    public bool IsGrounded { get; private set; }

    private void Update()
    {
        IsGrounded = Physics.SphereCast(transform.position, _groundDistance, Vector3.down, out _, _groundDistance, _groundLayers);
        if (IsGrounded == false && IsGrounded != previousState)
        {
            LeavingGround.Invoke();
        }
    }

    private void LateUpdate()
    {
        previousState = IsGrounded;
    }
}
