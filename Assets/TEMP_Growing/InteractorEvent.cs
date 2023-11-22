using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class InteractorEvent : MonoBehaviour
{
    public event Action<InteractorEvent, InteractorEventArgs> OnColorationFinished;

    public void CallInteractor()
    {
        OnColorationFinished?.Invoke(this, new InteractorEventArgs() { });
    }

}

public class InteractorEventArgs : EventArgs
{
    
}

