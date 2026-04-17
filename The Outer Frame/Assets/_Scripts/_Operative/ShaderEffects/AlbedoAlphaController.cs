using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlbedoAlphaController : MonoBehaviour
{
    Sequence FadeAlphaSequence;
    Material material;
    [SerializeField] float FadeDuration;
    float currentValue;
    float MaxValue;
    Coroutine FadeCoroutine;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        MaxValue = material.GetColor("_Color").a;
        currentValue = MaxValue;
    }

    public void FadeOutAlpha(Component sender, object obj)
    {
        if (FadeAlphaSequence != null && FadeAlphaSequence.IsPlaying())
        {
            FadeAlphaSequence.Kill();
        }

        FadeAlphaSequence = DOTween.Sequence();
        FadeAlphaSequence.Append(DOTween.To(() => currentValue, x => SetAlphaValue(x), 0f, FadeDuration)
                .SetEase(Ease.InOutSine));
    }

    public void FadeInAlpha(Component sender, object obj)
    {
        if (FadeAlphaSequence != null && FadeAlphaSequence.IsPlaying())
        {
            FadeAlphaSequence.Kill();
        }

        FadeAlphaSequence = DOTween.Sequence();
        FadeAlphaSequence.Append(DOTween.To(() => currentValue, x => SetAlphaValue(x), MaxValue, FadeDuration)
                .SetEase(Ease.InOutSine));
    }

    public void FadeForXSeconds(Component sender, object obj)
    {
        if (FadeCoroutine != null) StopCoroutine(FadeCoroutine);

        FadeCoroutine = StartCoroutine(FadeDelay((float)obj));
    }

    IEnumerator FadeDelay(float _duration)
    {
        FadeOutAlpha(null, null);
        yield return new WaitForSeconds(_duration);
        FadeInAlpha(null, null);

        FadeCoroutine = null;
    }

    void SetAlphaValue(float value)
    {
        currentValue = value;

        Color color = material.GetColor("_Color");
        color.a = value;

        material.SetColor("_Color", color);
    }

}
