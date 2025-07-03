using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoToPosition : MonoBehaviour
{
    [SerializeField] Transform ToPosition;
    [SerializeField] Vector3 ToPositionVector;
    [SerializeField] float time;

    public void goToPosition(Component sender, object obj)
    {
        Vector3 _position;
        if (ToPosition) _position = ToPosition.position;
        else _position = ToPositionVector;
        transform.DOMove(_position, time);
    }
}
