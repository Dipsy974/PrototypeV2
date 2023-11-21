using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderInteractorHolder : MonoBehaviour
{

    InteractorScript[] interactors;
    Vector4[] positions = new Vector4[1000];
    float[] radiuses = new float[1000];

    [Range(0, 1)]
    public float shapeCutoff;
    [Range(0, 1)]
    public float shapeSmoothness = .1f; 

    // Start is called before the first frame update
    void Start()
    {
        FindInteractors();
    }

    private void OnEnable()
    {
        
        FindInteractors();
    }

    private void Update()
    {
        FindInteractors(); 

        for (int i = 0; i < interactors.Length; i++)
        {
            positions[i] = interactors[i].transform.position;
            radiuses[i] = interactors[i].radius;
        }

        Shader.SetGlobalVectorArray("_ShaderInteractorsPositions", positions);
        Shader.SetGlobalFloatArray("_ShaderInteractorsRadiuses", radiuses); 

        Shader.SetGlobalFloat("_ShapeCutoff", shapeCutoff); 
        Shader.SetGlobalFloat("_ShapeSmoothness", shapeSmoothness); 

    }

    private void FindInteractors()
    {
        interactors = FindObjectsOfType<InteractorScript>(); 
    }
}
