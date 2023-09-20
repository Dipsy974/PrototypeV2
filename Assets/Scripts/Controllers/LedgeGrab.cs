using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;

public class LedgeGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    [SerializeField] private CharacterControlsInput _input;
    
    
    [SerializeField]
    private float _forwardOffset, _upwardOffset;
    
    //variables
    private bool _isHanging, _isDownLedgePressed, _canGrabLedge, _isLedgeClimbing, _canClimbLedge, _rbMoving;
    private Vector3 _positionToGrab, _directionToFace, _playerNewPosition, _playerStartPosition;
    
    private int _isHangingHash;
    private int _isLedgeClimbingHash;
    [SerializeField]private float _delta;

    private void Awake()
    {
        _isHangingHash = Animator.StringToHash("isHanging");
        _isLedgeClimbingHash = Animator.StringToHash("isLedgeClimbing");
        _canGrabLedge = true;
    }

    private void Update()
    {
        if (_characterController.IsFalling && _canGrabLedge)
        {
            CheckLedgeGrab();
        }

        if (_isHanging && _input.DownLedgeIsPressed )
        {
            ExitHang();
        }
        else if (_isHanging && _input.UpLedgeIsPressed)
        {
            _characterController.Animator.SetBool(_isLedgeClimbingHash, true);
            _isLedgeClimbing = true;
        }
    }

    private void FixedUpdate()
    {

    }


    private void CheckLedgeGrab()
    {
        
        Transform playerTransform = transform;
        
        //Downward Raycast in front of player to check for ground to grab and its y position
        RaycastHit downHit;
        Vector3 lineDownStart = (playerTransform.position + Vector3.up * 1.5f) + playerTransform.forward; //float multipliers need adjustement based on character's dimensions
        Vector3 lineDownEnd = (playerTransform.position + Vector3.up * 0.7f) + playerTransform.forward;
        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, LayerMask.GetMask("Walls"));
        Debug.DrawLine(lineDownStart,lineDownEnd);

        if (downHit.collider != null)
        {
            //Same but forward to get x and z of position to grab
            RaycastHit fwdHit;
            Vector3 lineFwdStart = new Vector3(playerTransform.position.x, downHit.point.y - 0.1f, playerTransform.position.z); 
            Vector3 lineFwdEnd = new Vector3(playerTransform.position.x, downHit.point.y - 0.2f, playerTransform.position.z) + playerTransform.forward;
            Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, LayerMask.GetMask("Walls"));
            Debug.DrawLine(lineFwdStart,lineFwdEnd);
            
            if (fwdHit.collider != null)
            {
                _positionToGrab = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
                _directionToFace = -fwdHit.normal;
                
                SnapToHangingPoint();
            }
        }
    }
    

    void SnapToHangingPoint()
    {
        _canGrabLedge = false;
        _characterController.CapCollider.enabled = false;
        _characterController.RB.isKinematic = true;
        _isHanging = true;
        _characterController.Animator.SetBool(_isHangingHash, true);
        _characterController.IsHanging = true;
        _characterController.IsJumping = false;
        _characterController.IsFalling = false;

        Vector3 hangPosition = _positionToGrab;
        Vector3 offset = transform.forward * _forwardOffset + transform.up * _upwardOffset; //offset adjusted to character dimensions
        hangPosition += offset;
        transform.position = hangPosition;
        transform.forward = _directionToFace;

        StartCoroutine(TriggerLedgeClimbCooldown());
    }
    
    private IEnumerator TriggerLedgeGrabCooldown()
    {
        _canGrabLedge = false;
        yield return new WaitForSeconds(0.8f);
        _canGrabLedge = true;
    }
    
    private IEnumerator TriggerLedgeClimbCooldown()
    {
        _canClimbLedge = false;
        yield return new WaitForSeconds(0.2f);
        _canClimbLedge = true;
    }

    private void ExitHang()
    {
        _characterController.CapCollider.enabled = true;
        _characterController.RB.isKinematic = false;
        _isHanging = false;
        _characterController.Animator.SetBool(_isHangingHash, false);
        _characterController.IsHanging = false;
        StartCoroutine(TriggerLedgeGrabCooldown());
    }

    private void EndLedgeClimb()  //Triggered at end of climb anim
    {
        _isLedgeClimbing = false;
        _characterController.Animator.SetBool(_isLedgeClimbingHash, false);
        
        //Reposition player
        _playerNewPosition = _positionToGrab + transform.up * 0f + transform.forward * 1f;
        _characterController.RB.position = _playerNewPosition;
        ExitHang();


    }
    
    
}
