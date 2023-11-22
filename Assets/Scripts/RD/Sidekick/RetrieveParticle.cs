using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrieveParticle : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        var throwSystem = other.transform.GetComponent<ThrowSystem>();
        if (throwSystem != null)
        {
            throwSystem.StopRetrieve();
        }
    }
}
