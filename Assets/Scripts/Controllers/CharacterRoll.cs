using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    
    [Header("Roll Variables")]
    private bool _isRollPressed, _isRolling, _rollDone, _canRoll;
    [SerializeField] private float rollSpeed, rollDistance, rollCooldown;
    private float _rollTime;
    private Vector3 _rollDirection;

    private void Awake()
    {
        
    }
    
    
}
