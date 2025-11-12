using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
    Image image;
    Color OriginalColor;
    [SerializeField] float Time;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void FadeOutImage(Component sender, object obj)
    {
        image.DOFade(0, Time);
    }


}
