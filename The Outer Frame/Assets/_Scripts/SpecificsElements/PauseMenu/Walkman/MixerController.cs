using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour, IDataPersistence
{
    [SerializeField] AudioMixer audiomixer;
    [SerializeField] string AudioMixerGroup;
    [SerializeField] TMP_Text textFiled;
    [SerializeField] bool isInverted; // true si el volumen va invertido (por ejemplo, sonido ambiente)

    float VolumeValue = 1f;
    bool isSettingInStart;

    private void Start()
    {
        isSettingInStart = true;
        if (float.TryParse(textFiled.text, out float initialTextValue))
        {
            float initialVolume = Mathf.Clamp(initialTextValue / 10f, 0.001f, 1f);
            SetVolume(null, initialVolume);
        }
        else
        {
            // fallback si el texto está vacío
            SetVolume(null, VolumeValue);
        }
    }

    public void SetVolume(Component sender, object obj)
    {
        VolumeValue = Mathf.Clamp((float)obj, 0.001f, 1f);
        float finalValue = isInverted ? 1 - VolumeValue : VolumeValue;

        float dB = Mathf.Log10(finalValue <= 0 ? 0.001f : finalValue) * 20;



        if (dB <= -59f)
        {
            audiomixer.SetFloat(AudioMixerGroup, -144);
        }
        else
        {
            audiomixer.SetFloat(AudioMixerGroup, dB);
        }

        if(slider)
        {
            slider.value = VolumeValue;
            lastSliderValue = VolumeValue;
        }

        isSettingInStart = false;
    }

    private float lastSliderValue;
    [SerializeField] Slider slider;
    public void VolumeSliderChanger(float sliderValue)
    {
        if (isSettingInStart) return;
        // Snap a divisiones de 0.1
        float currentValue = Mathf.Round(sliderValue * 10f) / 10f;

        // Cuántos pasos de 0.1 se movió
        int steps = Mathf.RoundToInt((currentValue - lastSliderValue) * 10f);

        if (steps != 0)
        {
            volumenChanger(steps * 0.1f);
        }

        // Guardamos el nuevo valor
        lastSliderValue = currentValue;

        // Aplicamos el snap visualmente
        slider.SetValueWithoutNotify(currentValue);
    }

    public void VolumeChanger(Component sender, object obj)
    {
        volumenChanger((float)obj);
    }

    public void volumenChanger(float amount)
    {
        VolumeValue += amount;
        VolumeValue = Mathf.Clamp(VolumeValue, 0.001f, 1f);

        float finalValue = isInverted ? 1 - VolumeValue : VolumeValue;
        float dB = Mathf.Log10(finalValue <= 0 ? 0.001f : finalValue) * 20;

        if (dB <= -59f)
        {
            audiomixer.SetFloat(AudioMixerGroup, -144f);
        }
        else
        {
            audiomixer.SetFloat(AudioMixerGroup, dB);
        }

        textFiled.text = Mathf.RoundToInt(VolumeValue * 10).ToString("00");

        DataPersistenceManager.instance.SaveGame();
    }

    // ---------------------
    // Persistencia de datos
    // ---------------------
    public void LoadData(GameData data)
    {
        if (isInverted)
            VolumeValue = data.SoundVolume;   // sonido
        else
            VolumeValue = data.MusicVolume;   // música

        float finalValue = isInverted ? 1 - VolumeValue : VolumeValue;
        float dB = Mathf.Log10(finalValue <= 0 ? 0.001f : finalValue) * 20;

        if (dB <= -59f)
        {
            audiomixer.SetFloat(AudioMixerGroup, -144f);
        }
        else
        {
            audiomixer.SetFloat(AudioMixerGroup, dB);
        }
        textFiled.text = Mathf.RoundToInt(VolumeValue * 10).ToString("00");
    }

    public void SaveData(GameData data)
    {
        if (isInverted)
            data.SoundVolume = VolumeValue;
        else
            data.MusicVolume = VolumeValue;
    }
}

