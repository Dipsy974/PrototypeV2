using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 position = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit; 

            if(Physics.Raycast(ray, out hit, 1000f))
            {
                Debug.Log("çatouche"); 
                TNTCPaintableObject paintable = hit.collider.GetComponent<TNTCPaintableObject>();
                if(paintable != null)
                {
                    PaintManager.instance.Paint(paintable, hit.point, 2f, 1f, 0.01f, Color.red); 
                }
            }
        }
    }
}
