using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    float radius;

    // Update is called once per frame
    void Update()
    {

        Shader.SetGlobalVector("_IPosition", transform.position);
        Shader.SetGlobalFloat("_IRadius", radius); 

    }
}