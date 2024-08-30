using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WritingShake : MonoBehaviour
{
    [SerializeField] Vector3 strength = new Vector3(90,90,90);
    [SerializeField] int vibrato = 10;
    [SerializeField] float randomness = 90;


    public void writingShake(Component sender, object obj)
    {
        float time = (float)obj;

        transform.DOShakeRotation(time, strength,vibrato,randomness,true,ShakeRandomnessMode.Harmonic);
        transform.DOShakeRotation(time, strength, vibrato, randomness, true, ShakeRandomnessMode.Harmonic);

    }
}
