using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorScript : MonoBehaviour
{
    [SerializeField]
    float radius;

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_InteractorPosition", transform.position);
        Shader.SetGlobalFloat("_RadiusInteractor", radius);
    }
}