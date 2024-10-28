using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SliderUpdateAnim : MonoBehaviour
{
    Slider slider;
    Sequence BarSequence;


    Sequence sequence;
    public void AnimSlider(Component sender, object obj)
    {
        if (sequence.IsActive()) sequence.Kill();

        slider = GetComponent<Slider>();
        slider.value = 0;

        sequence = DOTween.Sequence();

        float randomcharge = Random.Range(0.3f, 0.8f);

        sequence.AppendInterval(0.5f)
                .Append(slider.DOValue(randomcharge, 1.5f)).SetEase(Ease.Linear)
                .Append(slider.DOValue(1, 1f)).SetEase(Ease.Linear);
    }

    [ContextMenu("test Anim Slider")]
    void test()
    {
        AnimSlider(null, null);
    }
}
