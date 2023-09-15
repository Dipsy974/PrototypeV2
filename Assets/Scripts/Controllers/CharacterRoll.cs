using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoll : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    [SerializeField] private CharacterControlsInput _input;
    
    [Header("Roll Variables")]
    private bool _isRollPressed, _isRolling, _rollDone, _canRoll;
    [SerializeField] private float _rollForce, _rollDistance, _rollCooldown;
    private float _rollTime;
    private Vector3 _rollDirection;

    private void Awake()
    {
        _canRoll = true;
    }

    private void Update()
    {
        if (_input.RollIsPressed && _canRoll)
        {
            Debug.Log("roll");
            Roll();
        }
    }

    private void Roll()
    {

        _characterController.RB.AddForce(_characterController.transform.forward * _rollForce, ForceMode.Impulse);
        
    }
    
    private IEnumerator TriggerRollCooldown()
    {
        _canRoll = false;
        yield return new WaitForSeconds(_rollCooldown);
        yield return new WaitUntil(()=> !_input.RollIsPressed);
        _canRoll = true;
    }
}
