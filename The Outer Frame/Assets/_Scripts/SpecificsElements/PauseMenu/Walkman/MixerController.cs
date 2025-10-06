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
    [SerializeField] bool isInverted;
    float VolumeValue = 1;

    private void Start()
    {
       /* float mixerVolume;

        if (audiomixer.GetFloat(AudioMixerGroup, out mixerVolume))
        {
            VolumeValue = Mathf.Pow(10, mixerVolume / 20);
        }
        else
        {
            VolumeValue = 1;
        }

        textFiled.text = Mathf.RoundToInt((VolumeValue * 10)).ToString();*/
    }

    public void SetVolume(Component sender, object obj)
    {
        VolumeValue = (float)obj;
        float finalValue = isInverted ? 1 - VolumeValue : VolumeValue;

        audiomixer.SetFloat(AudioMixerGroup, Mathf.Log10(finalValue <= 0 ? 0.001f : finalValue) * 20);

        if(isInverted ) VolumeValue = 0;

    }

    public void VolumeChanger(Component sender, object obj)
    {
        VolumeValue = VolumeValue + (float)obj;

        VolumeValue = Mathf.Clamp(VolumeValue, 0.001f, 1);

        float finalValue = isInverted ? 1 - VolumeValue : VolumeValue;

        audiomixer.SetFloat(AudioMixerGroup, Mathf.Log10(finalValue <= 0 ? 0.001f : finalValue) * 20);

        textFiled.text = Mathf.RoundToInt((VolumeValue * 10)).ToString();

    }
}
