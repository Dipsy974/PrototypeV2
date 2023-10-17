using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(StM_PlayerController player) : base(player) { }
    public override void OnEnter()
    {
        Debug.Log("entering jump");
    }

    public override void FixedUpdate()
    {
        //HandleJump
        //_playerController.HandleMovement();
    }
    
    public override void OnExit()
    {
        Debug.Log("exiting jump");
    }
}
