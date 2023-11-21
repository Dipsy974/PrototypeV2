using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InteractorScript : MonoBehaviour
{
    
    public float radius;
    public float maxRadius;
    public bool isActive = false; 



    private void Start()
    {
        maxRadius = Random.Range(3f, 5f); 
    }

    // Update is called once per frame
    void Update()
    {
        if (radius < maxRadius)
        {
            radius = Mathf.Lerp(radius, maxRadius, 0.03f); 
        }
        //Shader.SetGlobalFloat("_RadiusInteractor", radius);
    }
}