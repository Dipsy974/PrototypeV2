using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "StateMachine/InputReader")]
public class StM_InputReader : ScriptableObject, PlayerInputActions.ICharacterControlsActions
{
    public event UnityAction<Vector2> Move = delegate {  };
    public event UnityAction<Vector2, bool> Look = delegate {  }; 
    public event UnityAction EnableMouseControlCamera = delegate {  };
    public event UnityAction DisableMouseControlCamera = delegate {  };

    private PlayerInputActions _inputActions;

    public Vector3 Direction => _inputActions.CharacterControls.Move.ReadValue<Vector2>();

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerInputActions();
            _inputActions.CharacterControls.SetCallbacks(this);
        }
        _inputActions.Enable();
    }

    public void EnablePlayerActions()
    {
        _inputActions.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControlCamera.Invoke();
                break;
        }
    }
}
