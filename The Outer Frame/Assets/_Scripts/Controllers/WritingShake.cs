using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WritingShake : MonoBehaviour
{
    [SerializeField] Vector3 strength = new Vector3(90,90,90);
    [SerializeField] int vibrato = 10;
    [SerializeField] float randomness = 90;
    Sequence DoShakeSequence;

    public void writingShake(Component sender, object obj)
    {
        float time = (float)obj;

        DoShakeSequence = DOTween.Sequence();

        if (DoShakeSequence != null && DoShakeSequence.IsActive()) DoShakeSequence.Kill();
        DoShakeSequence.Append(transform.DOShakeRotation(time, strength,vibrato,randomness,true,ShakeRandomnessMode.Harmonic))
                        .Join(transform.DOShakeRotation(time, strength, vibrato, randomness, true, ShakeRandomnessMode.Harmonic));

    }

    public void KillWriting(Component sender, object obj)
    {

    }
}
