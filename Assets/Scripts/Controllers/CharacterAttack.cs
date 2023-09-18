using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    //references
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    [SerializeField] private CharacterControlsInput _input;
    
    //private variables
    private bool _isAttackPressed = false;
    private bool _isAttacking = false;
    private bool _isComboFinished = false;  //issue when click is pressed at the end of the third attack: _isAttackingHash set to true but EndAttack not called because next animation not starting (click is pressed during transition). So I add a tiny CD at the end of combo
    private bool _isAttackAnimating;
    private int _attackCount = 0;
    private Coroutine _currentAttackResetRoutine = null;
    private int _isAttackingHash;
    private int _attackCountHash;

    private void Awake()
    {
        _isAttackingHash = Animator.StringToHash("isAttacking");
        _attackCountHash = Animator.StringToHash("attackCount");
    }

    private void Update()
    {
        HandleAttack();
    }


    private void HandleAttack()
    { 
        if (!_isAttacking && _input.AttackIsPressed && !_isComboFinished) //Attack only if _isComboFinished correctly reset
        {
            Debug.Log("attack");
            if (_attackCount < 3 && _currentAttackResetRoutine != null)
            {
                StopCoroutine(_currentAttackResetRoutine); //Stop the coroutine only if combo is not finished
            }

            _characterController.Animator.SetBool(_isAttackingHash, true);
            _isAttacking = true;
            _attackCount++;
            _characterController.Animator.SetInteger(_attackCountHash, _attackCount);
        }

    }
    
    private void EndAttack()  //Called in events in each attack animation settings
    {
        _characterController.Animator.SetBool(_isAttackingHash, false);
        _isAttacking = false;
        _currentAttackResetRoutine = StartCoroutine(attackResetRoutine());  //Set a coroutine to reset attack count after a delay at the end of an attack
        if (_attackCount >= 3) //Combo finished
        {
            _isComboFinished = true; //Set combo finished to true : player can't attack anymore until the attackResetRoutine set it to false
            _attackCount = 0;
            _characterController.Animator.SetInteger(_attackCountHash, _attackCount);
        }
    }

    
    private IEnumerator attackResetRoutine()  //Reset attack count but also reset _isComboFinished in case the combo is over
    {
        yield return new WaitForSeconds(.5f);
        _attackCount = 0;
        _isComboFinished = false;
    }
}
