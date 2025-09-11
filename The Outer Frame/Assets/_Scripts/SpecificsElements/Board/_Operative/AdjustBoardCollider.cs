using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustBoardCollider : MonoBehaviour
{
    BoxCollider _collider;
    Vector3 InitialPos;
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
        InitialPos = transform.position;
    }

    public void AdjustCollider(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if(view == ViewStates.BoardView || view == ViewStates.OnTakeSomeInBoard)
        {
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.position = InitialPos;
        }
    }
}
