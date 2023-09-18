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
    [SerializeField] private float _rollSpeed, _rollCooldown;
    [SerializeField] private float _rollTime;
    private Vector3 _rollDirection;

    private int _isRollingHash;


    private void Awake()
    {
        _isRollingHash = Animator.StringToHash("isRolling");
        _canRoll = true;
        float rollPlayrate = (_characterController.Animator.runtimeAnimatorController.animationClips[2].length/_rollTime); 
        _characterController.Animator.SetFloat("RollPlayrate", rollPlayrate);
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
        yield return new WaitForSeconds(_rollCooldown);
        yield return new WaitUntil(()=> !_input.RollIsPressed);
        _canRoll = true;
    }

    private IEnumerator EndRoll()
    {
        _characterController.Animator.SetBool(_isRollingHash, true);
        _canRoll = false;
        _isRolling = true;
        _characterController.IsRolling = true;
        yield return new WaitForSeconds(_rollTime);
        _isRolling = false;
        _characterController.IsRolling = false;
        _characterController.Animator.SetBool(_isRollingHash, false);
        StartCoroutine(TriggerRollCooldown());
    }
}
