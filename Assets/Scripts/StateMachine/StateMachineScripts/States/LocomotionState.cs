using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(StM_PlayerController player) : base(player) { }

    public override void OnEnter()
    {
        Debug.Log("entering locomotion");
    }

    public override void FixedUpdate()
    {
        //_playerController.HandleMovement();
    }
    
    public override void OnExit()
    {
        Debug.Log("exiting locomotion");
    }
}
