using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] AudioMixer audiomixer;
    [SerializeField] string AudioMixerGroup;

    public void SetVolume(float slidervalue)
    {
        audiomixer.SetFloat(AudioMixerGroup, Mathf.Log10(slidervalue) * 20);
    }
}
