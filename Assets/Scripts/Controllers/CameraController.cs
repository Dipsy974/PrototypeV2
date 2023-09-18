using UnityEngine;
using Cinemachine; 

public class CameraController : MonoBehaviour
{
    

    [SerializeField] private CharacterControlsInput _input;

    private CinemachineFreeLook _activeCamera;
    private int _activeCameraPriorityModifier = 100;

    public Camera mainCamera;
    public CinemachineFreeLook _thirdPersonCamera;
    public CinemachineFreeLook _focusCamera;


    //GETTERS AND SETTERS 
    public CinemachineFreeLook ActiveCamera { get { return _activeCamera; } private set { } }

    ////USELESS FOR NOW
    //public bool UsingOrbitalCamera { get; private set; } = false;
    //public CinemachineVirtualCamera _orbitCamera;

    private void Start()
    {
        SetFirstCamera();
    }

    private void Update()
    {
        if (_input.CameraChangeIsPressed) { ChangeCamera(); }
    }

    private void SetFirstCamera()
    {
        _thirdPersonCamera.Priority += _activeCameraPriorityModifier;
        _activeCamera = _thirdPersonCamera;
    }
    private void ChangeCamera()
    {
        if(_activeCamera == _thirdPersonCamera)
        {
            SetCameraPriority(_thirdPersonCamera, _focusCamera);
            _activeCamera.LookAt.GetChild(0).gameObject.SetActive(true);

        }
        else if(_activeCamera == _focusCamera)
        {
            _activeCamera.LookAt.GetChild(0).gameObject.SetActive(false);
            SetCameraPriority(_focusCamera, _thirdPersonCamera);
        }
    }

    private void SetCameraPriority(CinemachineFreeLook currentCameraMode, CinemachineFreeLook newCameraMode)
    {
        currentCameraMode.Priority -= _activeCameraPriorityModifier;
        newCameraMode.Priority += _activeCameraPriorityModifier;
        _activeCamera = newCameraMode; 
    }
}
