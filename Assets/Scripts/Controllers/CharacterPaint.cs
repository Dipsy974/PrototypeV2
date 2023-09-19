using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPaint : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NewCharacterLandController _characterController;
    [SerializeField] private CharacterControlsInput _input;

    [Header("Paint Variables")]
    public ParticleSystem paintSystem;

    private bool _isPainting = false; 



    private void Awake()
    {
        
    }

    private void Update()
    {
        paintSystem.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0.0f);

        if (_input.PaintIsPressed && !_isPainting)
        {
            Paint();
        }
        else if (!_input.PaintIsPressed)
        {
            StopPaint();
        }
    }

    private void Paint()
    {
        _isPainting = true; 
        paintSystem.Play();
    }
    
    private void StopPaint()
    {
        _isPainting = false;
        paintSystem.Stop();
    }
}
