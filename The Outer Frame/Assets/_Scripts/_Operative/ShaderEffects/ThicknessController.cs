using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ThicknessController : MonoBehaviour
{
    private TMP_Text textField;
    private Material mat;
    private Sequence thicknessSequence;

    private void Start()
    {
        textField = GetComponent<TMP_Text>();
        mat = GetComponent<Material>();
    }

    public void ThicknessOn(float targetValue = 0.5f, float duration = 0.25f)
    {
        // Cancelamos cualquier animación previa
        thicknessSequence?.Kill();

        thicknessSequence = DOTween.Sequence();

        thicknessSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_FaceDilate),      // getter
                x => mat.SetFloat(ShaderUtilities.ID_FaceDilate, x),   // setter
                targetValue,                                           // valor final
                duration                                               // duración
            ).SetEase(Ease.InOutSine)
        );
    }

    public void ThicknessOff(float endValue = 0f, float duration = 0.25f)
    {
        thicknessSequence?.Kill();

        thicknessSequence = DOTween.Sequence();

        thicknessSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_FaceDilate),
                x => mat.SetFloat(ShaderUtilities.ID_FaceDilate, x),
                endValue,
                duration
            ).SetEase(Ease.InOutSine)
        );
    }

}
