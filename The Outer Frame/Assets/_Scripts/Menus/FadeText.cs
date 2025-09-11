using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeText : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float FadeSpeed;
    [SerializeField] FadeType fadeType = FadeType.FadeIn;
    TMP_Text _text;

    enum FadeType
    {
        FadeIn,
        FadeOut
    }

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        if (fadeType == FadeType.FadeIn) _text.alpha = 0;
        else _text.alpha = 1;
    }

    public void Fadetext(Component sender, object obj)
    {
        if ((GameObject)obj != gameObject) return;
        if (fadeType == FadeType.FadeIn) _text.DOFade(1, FadeSpeed);
        else _text.DOFade(0, FadeSpeed);
    }

}
