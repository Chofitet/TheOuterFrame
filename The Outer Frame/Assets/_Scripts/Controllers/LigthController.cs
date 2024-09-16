using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LigthController : MonoBehaviour
{
    Light _light;
    [SerializeField] float attenuateFactor;
    private void Start()
    {
        _light = GetComponent<Light>();
    }

    public void AttenuateLigth(Component sender, object obj)
    {
        _light.DOIntensity(attenuateFactor, 2);
    }
}
