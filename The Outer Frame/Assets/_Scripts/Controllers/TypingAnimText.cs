using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TypingAnimText : MonoBehaviour
{
    TMP_Text TextField;

    int curretVisibleCharacterIndex;
    private Coroutine TypeCoroutine;

    WaitForSeconds delay;

    [SerializeField] float CharactersPerSecond = 20f;

    private void Awake()
    {
        TextField = GetComponent<TMP_Text>();
        delay = new WaitForSeconds(1 / CharactersPerSecond);
    }

    public void AnimateTyping()
    {
        if (TypeCoroutine != null) StopCoroutine(TypeCoroutine);

        TextField.maxVisibleCharacters = 0;
        TypeCoroutine = StartCoroutine(TypeWriter());
    }

    IEnumerator TypeWriter()
    {
        TMP_TextInfo textInfo = TextField.textInfo;

        while(curretVisibleCharacterIndex < textInfo.characterCount +1)
        {
            char character = textInfo.characterInfo[curretVisibleCharacterIndex].character;

            TextField.maxVisibleCharacters++;

            yield return delay;
        }
    }


    
}
