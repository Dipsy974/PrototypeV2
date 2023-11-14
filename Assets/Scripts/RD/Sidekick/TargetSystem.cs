using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    public List<Target> visibleTargets;
    public List<Target> reachableTargets;
    public float minReachDistance = 70;
    
    public Target currentTarget;
    public Target storedTarget;
    
    [SerializeField] float _screenDistanceWeight = 1;
    [SerializeField] float _positionDistanceWeight = 8;
    
    
}
