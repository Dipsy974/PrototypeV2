using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
    [SerializeField] private SelectionWheel _selectionWheel;

    private CharacterControlsInput _inputManager;

    private string _previousControlScheme = "";
    private const string _gamepadScheme = "Gamepad";
    private const string _mouseScheme = "KeyboardMouse";

    private bool _isOpenWheelPressed;

    private void Awake()
    {
        _inputManager = FindObjectOfType<CharacterControlsInput>();

        _inputManager.PlayerInput.UIControls.OpenWheel.performed += OnOpenWheel;
        _inputManager.PlayerInput.UIControls.OpenWheel.canceled += OnOpenWheel;
        
        _playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
    {
        if (_playerInput.currentControlScheme == _mouseScheme && _previousControlScheme != _mouseScheme)
        {
            Cursor.visible = true;
            _previousControlScheme = _mouseScheme;
        }
        else if (_playerInput.currentControlScheme == _gamepadScheme && _previousControlScheme != _gamepadScheme)
        {
            Cursor.visible = false;
            _previousControlScheme = _gamepadScheme;
        }
    }

    private void OnOpenWheel(InputAction.CallbackContext context)
    {
        _isOpenWheelPressed = context.ReadValueAsButton();
    }

    private void Update()
    {
        if (_isOpenWheelPressed)
        {
            _inputManager.PlayerInput.CharacterControls.Look.Disable();
            _selectionWheel.gameObject.SetActive(true);
            _inputManager.PlayerInput.CharacterControls.Disable();
            if (_playerInput.currentControlScheme == _mouseScheme)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        else
        {
            _inputManager.PlayerInput.CharacterControls.Look.Enable();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _selectionWheel.gameObject.SetActive(false);
            _inputManager.PlayerInput.CharacterControls.Enable();
        }
    }
    
    private void OnEnable()
    {
        _inputManager.PlayerInput.UIControls.Enable();
    }

    private void OnDisable()
    {
        _inputManager.PlayerInput.UIControls.Disable();
    }
}