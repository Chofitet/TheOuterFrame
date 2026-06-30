using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VolumenSettings : MonoBehaviour, IDataPersistence
{
    public AudioMixer mixer;
    public string parameterName = "MasterVolume";
    [SerializeField] float duration;
    private Tween tween;
    [SerializeField] bool fadeOnStart;
    [SerializeField] bool isInverted;
    [SerializeField] bool fadeOnStartToSavedVolume;

    private void Start()
    {
        if (fadeOnStart) FadeIn();
        if (fadeOnStartToSavedVolume) FadeInToSavedVolume();
    }

    public void OnFadeIn(Component sender, object obj)
    {
        FadeIn();
    }
    public void OnFadeOut(Component sender, object obj)
    {
        FadeOut();
    }
    public void FadeIn()
    {
        FadeTo(0f, duration); // 0 dB = volumen normal
    }

    public void FadeInToSavedVolume()
    {
        float finalValue = isInverted ? 1 - SavedVolumeValue : SavedVolumeValue;
        float dB = Mathf.Log10(finalValue <= 0 ? 0.001f : finalValue) * 20;


        if (dB <= -59f)
        {
            FadeTo(-144, duration);
        }
        else
        {
            FadeTo(dB, duration); 
        }

        
    }

    public void FadeOut()
    {
        FadeTo(-80f, duration); // -80 dB = silencio
    }

    private void FadeTo(float targetDb, float duration)
    {
        // Cancelar tween anterior si lo hubiera
        tween?.Kill();

        // Obtener valor actual
        mixer.GetFloat(parameterName, out float currentDb);

        tween = DOTween.To(
            () => currentDb,
            x => mixer.SetFloat(parameterName, x),
            targetDb,
            duration
        );
    }

    private void OnDestroy()
    {
        tween?.Kill();
        tween = null;
    }

    float SavedVolumeValue = 1f;
    public void LoadData(GameData data)
    {
        SavedVolumeValue = data.SoundVolume;
    }

    public void SaveData(GameData data)
    {
        
    }
}
