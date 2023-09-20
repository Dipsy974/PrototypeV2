using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    [SerializeField] private CharacterControlsInput _input;
    
    
    [SerializeField]
    private float _forwardOffset, _upwardOffset;
    
    //variables
    private bool _isHanging, _isDownLedgePressed, _canGrabLedge;
    private Vector3 _positionToGrab, _directionToFace;
    
    private int _isHangingHash;

    private void Awake()
    {
        _isHangingHash = Animator.StringToHash("isHanging");
        _canGrabLedge = true;
    }

    private void Update()
    {
        if (_characterController.IsFalling && _canGrabLedge)
        {
            CheckLedgeGrab();
        }

        if (_isHanging && _input.DownLedgeIsPressed)
        {
            ExitHang();
        }
    }


    private void CheckLedgeGrab()
    {
        Debug.Log("check ledge");
        
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
        Debug.Log("Pos to hang : " + transform.position);

        transform.forward = _directionToFace;
    }
    
    private IEnumerator TriggerLedgeGrabCooldown()
    {
        _canGrabLedge = false;
        yield return new WaitForSeconds(0.2f);
        _canGrabLedge = true;
    }

    private void ExitHang()
    {
        Debug.Log("exit");
        _characterController.CapCollider.enabled = true;
        _characterController.RB.isKinematic = false;
        _isHanging = false;
        _characterController.Animator.SetBool(_isHangingHash, false);
        _characterController.IsHanging = false;
        StartCoroutine(TriggerLedgeGrabCooldown());
    }
}