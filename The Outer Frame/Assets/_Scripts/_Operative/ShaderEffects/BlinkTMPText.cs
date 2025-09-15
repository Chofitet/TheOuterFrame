using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkTMPText : MonoBehaviour
{
    Material material;
    [SerializeField] float duration = 0.5f;
    [SerializeField] float TurnOffDuration;
    [SerializeField] float maxIntensity = 1f;  // Máxima intensidad para el glow
    [SerializeField] bool BlinkInStart;
    [SerializeField] float TurnLightDuration;
    Sequence BlinkSequence;
    private float currentIntensity = 0f;  // Estado actual de la intensidad del glow

    void OnEnable()
    {
        if (TurnOffDuration == 0) TurnOffDuration = duration;

        // Intentar obtener el material de TMP_Text
        TMP_Text tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            material = tmpText.fontSharedMaterial;
        }
        else if (GetComponent<Renderer>())
        {
            material = GetComponent<Renderer>().material;
        }
        else return;

        // Iniciar el parpadeo si está configurado para comenzar automáticamente
        if (BlinkInStart) ActiveBlink(null, null);
        else
        {
            SetGlowIntensity(0);
        }
    }

    // Establecer la intensidad del glow (solo modifica el _GlowPower)
    void SetGlowIntensity(float intensity)
    {
        currentIntensity = intensity;
        material.SetFloat("_GlowPower", intensity);  // Ajustar solo la intensidad del glow
    }

    public void ActiveBlink(Component sender, object obj)
    {
        if (material.HasProperty("_GlowPower"))
        {
            BlinkSequence = DOTween.Sequence();

            // Ajustar la intensidad del glow entre 0 y maxIntensity
            BlinkSequence.Append(DOTween.To(() => 0f, x => SetGlowIntensity(x), maxIntensity, duration))
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            BlinkSequence.Play();
        }
        else
        {
            Debug.LogWarning("El material no tiene la propiedad '_GlowPower'.");
        }
    }

    public void TurnOffLight(Component sender, object obj)
    {
        if (BlinkSequence != null && BlinkSequence.IsPlaying())
        {
            BlinkSequence.Kill();
        }

        BlinkSequence = DOTween.Sequence();
        BlinkSequence.Append(DOTween.To(() => currentIntensity, x => SetGlowIntensity(x), 0f, TurnOffDuration)
                .SetEase(Ease.OutExpo));
    }

    public void TurnOnLigth(Component sender, object obj)
    {
        if (BlinkSequence != null)
        {
            BlinkSequence.Kill();
        }

        BlinkSequence = DOTween.Sequence();
        BlinkSequence.Append(DOTween.To(() => currentIntensity, x => SetGlowIntensity(x), maxIntensity, duration)
            .SetEase(Ease.InExpo));
    }

    public void TurnForXSeconds(Component sender, object obj)
    {
        StartCoroutine(turnLigthDelay());
    }

    IEnumerator turnLigthDelay()
    {
        TurnOnLigth(null, null);
        yield return new WaitForSeconds(TurnLightDuration);
        TurnOffLight(null, null);
    }

    [ContextMenu("TestBlink")]
    private void TestBlink()
    {
        ActiveBlink(null, null);
    }

}
