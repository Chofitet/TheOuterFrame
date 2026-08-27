using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachinePropertiesController : MonoBehaviour
{
    CinemachineVirtualCamera Vcam;
    CinemachineBasicMultiChannelPerlin noise;

    [SerializeField] Vector3 noisePivotOffset;
    [SerializeField] float noiseAmplitudeGain;
    [SerializeField] float noiseFrequencyGain;

    private void Start()
    {
        Vcam = GetComponent<CinemachineVirtualCamera>();
        noise = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ChangeNoisePivotOffset(Component sender, object obj)
    {
        if(obj == null || obj is not Vector3)
        {
            noise.m_PivotOffset = noisePivotOffset;
        }
        else
        {
            noise.m_PivotOffset = (Vector3)obj;
        }
    }

    public void ChangeNoiseAmplitudeGain(Component sender, object obj)
    {
        if (obj == null || obj is not float)
        {
            noise.m_AmplitudeGain = noiseAmplitudeGain;
        }
        else
        {
            noise.m_AmplitudeGain = (float)obj;
        }
    }


    public void ChangeNoiseFrecuency(Component sender, object obj)
    {
        if (obj == null || obj is not float)
        {
            noise.m_FrequencyGain = noiseFrequencyGain;
        }
        else
        {
            noise.m_FrequencyGain = (float)obj;
        }
    }

}
