using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControlsInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public bool MoveIsPressed { get; private set; } = false;
    
    public Vector2 LookInput { get; private set; } = Vector2.zero;
    public bool InvertMouseY { get; private set; } = true;

    public bool CameraChangeIsPressed { get; private set; } = false; 

    InputActions _input;

    private void OnEnable()
    {
        _input = new InputActions();
        _input.CharacterControls.Enable();

        _input.CharacterControls.Move.performed += SetMove;
        _input.CharacterControls.Move.canceled += SetMove;

        _input.CharacterControls.Look.performed += SetLook;
        _input.CharacterControls.Look.canceled += SetLook;
    }

    private void OnDisable()
    {
        _input.CharacterControls.Move.performed -= SetMove;
        _input.CharacterControls.Move.canceled -= SetMove;

        _input.CharacterControls.Look.performed -= SetLook;
        _input.CharacterControls.Look.canceled -= SetLook;

        _input.CharacterControls.Disable();
    }

    private void Update()
    {
        CameraChangeIsPressed = _input.CharacterControls.ChangeCamera.WasPressedThisFrame(); 
    }

    private void SetMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
        MoveIsPressed = !(MoveInput == Vector2.zero); 
    }

    private void SetLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }
}
