using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateController : MonoBehaviour
{
    [SerializeField] Transform rotateToTransform;
    [SerializeField] Vector3 rotateTo;
    [SerializeField] float timeToRotate;

    public void RotateTo(Component sender, object obj)
    {
        Vector3 AngleToRotate;
        if (rotateToTransform) AngleToRotate = rotateToTransform.rotation.eulerAngles;
        else AngleToRotate = rotateTo;
        transform.DORotate(AngleToRotate, timeToRotate).SetEase(Ease.InOutSine);
    }
}
