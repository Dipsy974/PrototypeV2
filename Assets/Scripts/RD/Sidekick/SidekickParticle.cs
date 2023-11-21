using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidekickParticle : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        var target = other.transform.GetComponent<Target>();
        if (target != null)
        {
            target.OnActivate();
        }
    }
}
