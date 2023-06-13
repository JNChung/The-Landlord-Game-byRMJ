using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UiComponent 
{
    public bool Selected { get; }
    public ObjectType GetObjectType();
}
