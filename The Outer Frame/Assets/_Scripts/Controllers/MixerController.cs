using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] AudioMixer audiomixer;
    [SerializeField] string AudioMixerGroup;
    [SerializeField] TMP_Text textFiled;
    float VolumeValue = 1;

    public void VolumeChanger(Component sender, object obj)
    {
        VolumeValue = VolumeValue + (float)obj;
        VolumeValue = Mathf.Clamp(VolumeValue, 0.001f, 1);
        audiomixer.SetFloat(AudioMixerGroup, Mathf.Log10(VolumeValue) * 20);
        textFiled.text = Mathf.RoundToInt((VolumeValue * 10)).ToString();
    }
}
