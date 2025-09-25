using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FadeTextsController : MonoBehaviour
{
    [SerializeField] GameEvent OnFadeText;
    [SerializeField] List<TextData> TextAnimationData = new List<TextData>();
    [SerializeField] GameEvent OnChangeLevel;


    private void Start()
    {
        StartCoroutine(HandleFades());
    }

    private IEnumerator HandleFades()
    {
        foreach (var textData in TextAnimationData)
        {
            
            yield return new WaitForSeconds(textData.TimeToStartFade);

            OnFadeText?.Invoke(this, textData.text);

        }

        yield return new WaitForSeconds(3);
        OnChangeLevel?.Invoke(this, null);
    }
}

[Serializable]
class TextData
{
    public GameObject text;
    public float TimeToStartFade;
}
