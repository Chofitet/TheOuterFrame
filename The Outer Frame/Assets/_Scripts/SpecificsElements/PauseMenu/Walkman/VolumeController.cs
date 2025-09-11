using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] float fadeDuration = 1f;

    private float originalVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Almacena el volumen original al inicio
        if (audioSource != null)
        {
            originalVolume = audioSource.volume;
            audioSource.volume = 0;
        }
    }

    // Función para bajar el volumen a cero
    public void FadeOutVolume(Component sender, object obj)
    {
        if (audioSource != null)
        {
            audioSource.DOFade(0f, fadeDuration);
        }
    }

    // Función para restaurar el volumen original
    public void FadeInVolume(Component sender, object obj)
    {
        if (audioSource != null)
        {
            audioSource.DOFade(originalVolume, fadeDuration);
        }
    }

    public void ChangeFadeOut(Component sender, object obj)
    {

        fadeDuration = (float)obj;
    }

    bool isInTutorial;
    public void setTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;

        if(!isInTutorial)
        {
            audioSource.Play();
            audioSource.volume = 0;
            FadeInVolume(null, 0.5f);
        }
    }
}
