using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparentObject : MonoBehaviour
{
    [SerializeField] GameObject _object;
    [SerializeField] Transform OriginalParent;
    [SerializeField] Transform ToReparent;
    bool x;

    public void reparentObject(Component sender, object obj)
    {
        if(!x)
        {
            _object.transform.SetParent(ToReparent);
            _object.transform.position = Vector3.zero;
            x = true;
        }
        else
        {
            _object.transform.SetParent(OriginalParent, true);
            _object.transform.position = Vector3.zero;
        }
            

    }
}
