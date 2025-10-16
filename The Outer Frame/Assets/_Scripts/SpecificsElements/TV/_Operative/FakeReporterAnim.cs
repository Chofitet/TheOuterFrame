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
    float speedFactor = 1f;
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

        float adjustedWait = Random.Range(MinWaitTime, MaxWaitTime) / speedFactor;
        float adjustedDuration = RotationDuration / speedFactor;

        float targetZ = Random.Range(MinAngle, MaxAngle);

        rotateSequence.AppendInterval(adjustedWait) // pequeño delay inicial
            .AppendCallback(() =>
            {
                float targetZ = Random.Range(MinAngle, MaxAngle);
                Reporter.DOLocalRotate(new Vector3(Reporter.localEulerAngles.x, Reporter.localEulerAngles.z, targetZ), adjustedDuration)
                    .SetEase(Ease.InOutCubic)
                    .OnComplete(() => RotateRandomly());
            });
    }

    public void AccelerateAnimator(Component sender, object obj)
    {
        speedFactor = (float)obj;
        RotateRandomly();

    }
}
