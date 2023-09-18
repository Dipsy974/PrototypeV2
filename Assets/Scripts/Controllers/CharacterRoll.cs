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
    [SerializeField] private float _rollSpeed, _rollDistance, _rollCooldown;
    [SerializeField] private float _rollTime;
    private Vector3 _rollDirection;


    private void Awake()
    {
        _canRoll = true;
        //_rollTime = _rollDistance / _rollSpeed;
    }

    private void Update()
    {
        if (_input.RollIsPressed && _canRoll)
        {
            StartCoroutine(Roll());
        }
    }

    private IEnumerator Roll()
    {
        StartCoroutine(EndRoll());
        while (_isRolling)
        {
            _characterController.RB.AddForce(_characterController.transform.forward * _rollSpeed, ForceMode.Force);
            yield return new WaitForEndOfFrame();
        }
    }
    
    private IEnumerator TriggerRollCooldown()
    {
        _canRoll = false;
        yield return new WaitForSeconds(_rollCooldown);
        yield return new WaitUntil(()=> !_input.RollIsPressed);
        _canRoll = true;
    }

    private IEnumerator EndRoll()
    {
        _isRolling = true;
        yield return new WaitForSeconds(_rollTime);
        _isRolling = false;
        StartCoroutine(TriggerRollCooldown());
    }
}
