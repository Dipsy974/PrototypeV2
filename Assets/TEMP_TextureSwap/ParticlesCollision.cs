using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCollision : MonoBehaviour
{
    [Space]
    ParticleSystem part;
    List<ParticleCollisionEvent> collisionEvents;

    public InteractorScript interactorPrefab; 

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

    }

    void OnParticleCollision(GameObject other)
    {
        //int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        //InteractorObject interactorObj = other.GetComponent<InteractorObject>();

        //if(interactorObj != null)
        //{
        //    //for (int i = 0; i < numCollisionEvents; i++)
        //    //{
        //    //    Vector3 pos = collisionEvents[i].intersection;
        //    //    Instantiate(interactorPrefab, pos, Quaternion.identity); 
        //    //}
        //    Debug.Log(interactorObj.transform.parent.gameObject);

        //    GameObject parent = interactorObj.transform.root; 
        //    interactorObj.SetObjectActive(); 
        //}
        

    }
}