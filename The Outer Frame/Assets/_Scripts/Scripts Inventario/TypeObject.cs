using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectType
{
    glassdarkstuff,
    glass,
    glasswater,
    teeth,
    crayon,
    crayonbox,
    shoe,
    coathanger,
    hook,
    file,
    flesh,
    nothing,
}

public class TypeObject : MonoBehaviour
{
    [SerializeField] ObjectType objectType;
   
    public ObjectType GetObjectType()
    {
        return objectType;
    }
    private void Update()
    {

    }
}
