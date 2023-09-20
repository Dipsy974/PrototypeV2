using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisplayPaint : MonoBehaviour
{
    [SerializeField] GameObject decalProjectorPrefab;
    [SerializeField] GameObject effectCollider;
    List<ParticleCollisionEvent> collisionEvents;

    [SerializeField] private int particlesHitLimit = 5;
    private int particlesHitCounter; 

    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>(); 
    }
    private void OnParticleCollision(GameObject other)
    {

        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);
        particlesHitCounter++; 

        if(particlesHitCounter >= 5)
        {
            particlesHitCounter = 0; 
            PaintOn(collisionEvents[0]);
        }

    }
   

    private void PaintOn(ParticleCollisionEvent particleCollisionEvent)
    {
        Debug.Log(particleCollisionEvent.intersection); 
        var decal = Instantiate(decalProjectorPrefab, particleCollisionEvent.intersection, Quaternion.LookRotation(-particleCollisionEvent.normal)); 
        var effect = Instantiate(effectCollider, particleCollisionEvent.intersection, Quaternion.LookRotation(Vector3.zero));
        effect.transform.rotation = Quaternion.FromToRotation(effect.transform.up, particleCollisionEvent.normal); 
    }

    public void SetPaintElements(GameObject decal, GameObject effect)  //Called by PaintManager to change decals and effects Collider
    {
        decalProjectorPrefab = decal;
        effectCollider = effect; 
    }

}
