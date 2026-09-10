using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] GameObject Object;
    [SerializeField] float strenght;
    [SerializeField] int vibrato;
    [SerializeField] float randomness;
    [SerializeField] ShakeRandomnessMode randomnessMode;

    [ContextMenu("On Shake")]
    public void OnShakeTest()
    {
        OnShake(null,null);
    }
    public void OnShake(Component sender,object obj)
    {
        Object.transform.DOShakeRotation(1000000, strenght,vibrato, randomness,true, randomnessMode);
    }
}
