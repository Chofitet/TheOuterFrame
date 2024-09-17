using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BlinkMaterialEffect : MonoBehaviour
{
    Material material; 
    [SerializeField] float duration = 0.5f; 
    [SerializeField] float maxIntensity = 1f;
    [SerializeField] bool BlinkInStart;
    [SerializeField] float TurnLightDuration;
    private Color originalEmissionColor;
    private Color _color;
    [ColorUsage(true, true)]
    [SerializeField] Color otherColorTochange;
    Sequence BlinkSequence;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        originalEmissionColor = material.GetColor("_EmissionColor");
        _color = originalEmissionColor;
        if (BlinkInStart) ActiveBlink(null, null);
        else
        {
            SetEmissionIntensity(0);
        }
    }

    void SetEmissionIntensity(float intensity)
    {
        Color emissionColor = _color * intensity;
        material.SetColor("_EmissionColor", emissionColor);
    }
    public void ActiveBlink(Component sender, object obj)
    {
        if (material.IsKeywordEnabled("_EMISSION"))
        {
            
            BlinkSequence = DOTween.Sequence();

            BlinkSequence.Append(DOTween.To(() => 0f, x => SetEmissionIntensity(x), maxIntensity, duration))
                .SetLoops(-1, LoopType.Yoyo) 
                .SetEase(Ease.InOutSine); 

            BlinkSequence.Play();
        }
        else
        {
            Debug.LogWarning("El material no tiene la propiedad '_EmissionColor' activada.");
        }
    }

    public void TurnOffLight(Component sender, object obj)
    {

        if (BlinkSequence != null && BlinkSequence.IsPlaying())
        {
            BlinkSequence.Kill();
        }

        DOTween.To(() => maxIntensity, x => SetEmissionIntensity(x), 0f, duration)
                .SetEase(Ease.InOutSine);

        
    }

    public void TurnOnLigth(Component sender, object obj)
    {
        if (BlinkSequence != null)
        {
            BlinkSequence.Kill();
        }

        DOTween.To(() => 0f, x => SetEmissionIntensity(x), maxIntensity, duration)
            .SetEase(Ease.InOutSine);
    }

    public void SetOtherColor(Component sender, object obj)
    {
        _color = otherColorTochange;
    }

    public void SetOriginalColor(Component sender, object obj)
    {
        _color = originalEmissionColor;
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
