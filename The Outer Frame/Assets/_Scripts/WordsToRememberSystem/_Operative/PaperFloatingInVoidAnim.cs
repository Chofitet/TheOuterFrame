using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaperFloatingInVoidAnim : MonoBehaviour
{
    [SerializeField] float angle = 5f;
    [SerializeField]  float duration = 4f;
    [SerializeField] int RotateDirection = 1;
    private float currentAngle;
    float Auxangle;
    private void Start()
    {
        currentAngle = 0;
        Auxangle = angle * RotateDirection;

        StartFloatingCycle();
    }

    void StartFloatingCycle()
    {
        DOTween.To(
            () => currentAngle,
            x =>
            {
                currentAngle = x;
                transform.localRotation = Quaternion.Euler(0, 0, x);
            },
            Auxangle,
            duration
        )
        .SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            Auxangle = -Auxangle;
            StartFloatingCycle();
        });
    }
}
