using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControlsInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public bool MoveIsPressed { get; private set; } = false;
    
    public Vector2 LookInput { get; private set; } = Vector2.zero;
    public bool InvertMouseY { get; private set; } = true;

    public bool CameraChangeIsPressed { get; private set; } = false; 
    
    public bool RollIsPressed { get; private set; } = false; 
    public bool JumpIsPressed { get; private set; } = false; 
    public bool PaintIsPressed { get; private set; } = false; 
    
    public bool AttackIsPressed { get; private set; } = false; 
    
    public bool DownLedgeIsPressed { get; private set; } = false; 
    
    public bool UpLedgeIsPressed { get; private set; } = false; 

    InputActions _input;
    
    public InputActions PlayerInput { get { return _input; } }

    private void OnEnable()
    {
        _input = new InputActions();
        _input.CharacterControls.Enable();

        _input.CharacterControls.Move.performed += SetMove;
        _input.CharacterControls.Move.canceled += SetMove;

        _input.CharacterControls.Look.performed += SetLook;
        _input.CharacterControls.Look.canceled += SetLook;

        _input.CharacterControls.Jump.started += SetJump;
        _input.CharacterControls.Jump.canceled += SetJump;
        
        _input.CharacterControls.Attack.started += SetAttack;
        _input.CharacterControls.Attack.canceled += SetAttack;

        _input.CharacterControls.Paint.started += SetPaint;
        _input.CharacterControls.Paint.canceled += SetPaint;
        
        _input.CharacterControls.DownLedge.started += SetDownLedge;
        _input.CharacterControls.DownLedge.performed += SetDownLedge;
        _input.CharacterControls.DownLedge.canceled += SetDownLedge;
        
        _input.CharacterControls.UpLedge.started += SetUpLedge;
        _input.CharacterControls.UpLedge.performed += SetUpLedge;
        _input.CharacterControls.UpLedge.canceled += SetUpLedge;
    }

    private void OnDisable()
    {
        _input.CharacterControls.Move.performed -= SetMove;
        _input.CharacterControls.Move.canceled -= SetMove;

        _input.CharacterControls.Look.performed -= SetLook;
        _input.CharacterControls.Look.canceled -= SetLook;

        _input.CharacterControls.Jump.performed -= SetJump;
        _input.CharacterControls.Jump.canceled -= SetJump;
        
        _input.CharacterControls.Attack.performed -= SetAttack;
        _input.CharacterControls.Attack.canceled -= SetAttack;

        _input.CharacterControls.Paint.started -= SetPaint;
        _input.CharacterControls.Paint.canceled -= SetPaint;
        
        _input.CharacterControls.DownLedge.started -= SetDownLedge;
        _input.CharacterControls.DownLedge.performed -= SetDownLedge;
        _input.CharacterControls.DownLedge.canceled -= SetDownLedge;

        _input.CharacterControls.Disable();
    }

    private void Update()
    {
        CameraChangeIsPressed = _input.CharacterControls.ChangeCamera.WasPressedThisFrame(); 
        RollIsPressed = _input.CharacterControls.Roll.WasPressedThisFrame();
    }

    private void SetMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        MoveIsPressed = !(MoveInput == Vector2.zero); 
    }

    private void SetJump(InputAction.CallbackContext ctx)
    {
        JumpIsPressed = ctx.started;
    }
    
    private void SetAttack(InputAction.CallbackContext ctx)
    {
        AttackIsPressed = ctx.started;
    }

    private void SetPaint(InputAction.CallbackContext ctx)
    {
        PaintIsPressed = ctx.started;
    }

    private void SetLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }
    
    private void SetDownLedge(InputAction.CallbackContext ctx)
    {
        DownLedgeIsPressed = ctx.ReadValueAsButton();
    }
    
    private void SetUpLedge(InputAction.CallbackContext ctx)
    {
        UpLedgeIsPressed = ctx.ReadValueAsButton();
    }
}
