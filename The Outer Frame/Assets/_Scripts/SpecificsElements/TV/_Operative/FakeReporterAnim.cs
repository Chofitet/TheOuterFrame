using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeReporterAnim : MonoBehaviour
{
    [SerializeField] float MinAngle;
    [SerializeField] float MaxAngle;
    [SerializeField] float RotationDuration = 1.5f;
    [SerializeField] float MinWaitTime;
    [SerializeField] float MaxWaitTime;
    [SerializeField] Transform Reporter;
    Sequence rotateSequence;
    private void OnEnable()
    {
        RotateRandomly();
    }

    private void OnDisable()
    {
        if (rotateSequence != null && rotateSequence.IsActive())
            rotateSequence.Kill();
    }

    void RotateRandomly()
    {
        if (rotateSequence != null && rotateSequence.IsActive())
            rotateSequence.Kill();

        rotateSequence = DOTween.Sequence();

        float targetZ = Random.Range(MinAngle, MaxAngle);
        float WaitTime = Random.Range(MinWaitTime, MaxWaitTime);

        rotateSequence.AppendInterval(WaitTime) // pequeño delay inicial
            .AppendCallback(() =>
            {
                float targetZ = Random.Range(MinAngle, MaxAngle);
                Reporter.DOLocalRotate(new Vector3(Reporter.localEulerAngles.x, Reporter.localEulerAngles.z, targetZ), RotationDuration)
                    .SetEase(Ease.InOutCubic)
                    .OnComplete(() => RotateRandomly());
            });
    }
}
