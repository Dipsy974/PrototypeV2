using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorableObject : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetObjectActive()
    {
        GetComponentInChildren<InteractorScript>().isActive = true;

        if (GetComponent<GrowVines>())
        {

        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("isActive", true);
        }

    }

}



