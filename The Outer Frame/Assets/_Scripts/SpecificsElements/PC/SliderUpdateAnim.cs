using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SliderUpdateAnim : MonoBehaviour
{
    Slider slider;
    Sequence BarSequence;
    [SerializeField] float duration;

    Sequence sequence;
    public void AnimSlider(Component sender, object obj)
    {

        if (sequence.IsActive()) sequence.Kill();

        slider = GetComponent<Slider>();
        slider.value = 0;

        sequence = DOTween.Sequence();

        float randomcharge = Random.Range(0.3f, 0.8f);

        sequence.AppendInterval(0.2f)
                .Append(slider.DOValue(randomcharge, duration)).SetEase(Ease.Linear)
                .Append(slider.DOValue(1, duration/2)).SetEase(Ease.Linear);
    }

    [ContextMenu("test Anim Slider")]
    void test()
    {
        AnimSlider(null, null);
    }

}
