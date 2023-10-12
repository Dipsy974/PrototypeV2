using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBlend : MonoBehaviour
{
    private Material material;
 

    // Start is called before the first frame update
    void Start()
    {
        var renderer = GetComponent<SkinnedMeshRenderer>();
        material = Instantiate(renderer.sharedMaterial);
        renderer.material = material; 
        
     
    }


    private void OnDestroy()
    {
        if(material != null)
        {
            Destroy(material); 
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastClickRay();
        }
    }

    private void CastClickRay()
    {
        var camera = Camera.main;
        var mousePos = Input.mousePosition;

        var ray = camera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane));

        Debug.Log("Cliqué");
        if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject)
        {
  
            StartRipple(hit.point);
            Debug.Log(mousePos); 
        }
    }

    private void StartRipple(Vector3 center)
    {
        Debug.Log("Touché"); 
        material.SetVector("_RippleCenter", center);
        material.SetFloat("_RippleStart", Time.time); 
    }
}
