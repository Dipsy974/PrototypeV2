using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{

    private GameObject surfaceOn; 
    // Start is called before the first frame update
    void Awake()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetSurface(GameObject surface)
    {
        surfaceOn = surface; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NewCharacterLandController>() != null)
        {
            NewCharacterLandController player = other.GetComponent<NewCharacterLandController>();

            player.CanBounce = true; 

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<NewCharacterLandController>() != null)
        {
            NewCharacterLandController player = other.GetComponent<NewCharacterLandController>();

            player.CanBounce = false;
        }
    }
}
