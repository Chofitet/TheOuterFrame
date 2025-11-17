using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VolumenSettings : MonoBehaviour
{
    public AudioMixer mixer;
    public string parameterName = "MasterVolume";
    [SerializeField] float duration;
    private Tween tween;

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



}
