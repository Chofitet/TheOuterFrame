using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Searcher;
using UnityEngine;

public class SearchBarController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBarText;
    [SerializeField] TMP_Text WaitingSearchBarText;
    bool isWaitingAWord;
    Coroutine IdleSearchBarAnimCoroutine;
    string initWaitingSearchBarText;
    TypingAnimText textAnim;
    private void Awake()
    {
        textAnim = SearchBarText.GetComponent<TypingAnimText>();
        textAnim.SetCharacterPerSecond(2);
        initWaitingSearchBarText = WaitingSearchBarText.text;
    }
    public void EnterAWord(WordData word)
    {
        SearchBarText.text = " " + word.GetName().Replace("?", "").Replace("!", "");
        SearchBarText.GetComponent<TypingAnimText>().AnimateTyping();
    }

    public void ClearWord()
    {
        SearchBarText.text = " |";
    }

    public void StartIdleSearchBarAnim()
    {
        if (gameObject.activeInHierarchy) IdleSearchBarAnimCoroutine = StartCoroutine(IdleSearchBarAnim());
    }

    public void StopIdleSearchBarAnim()
    {
        if(IdleSearchBarAnimCoroutine != null) StopCoroutine(IdleSearchBarAnimCoroutine);
    }

    public void SetCharacterPerSecond()
    {
        WaitingSearchBarText.text = "";
        textAnim.SetCharacterPerSecond();
    }

    public void SetIsAwaitingAWord(bool x)
    {
        isWaitingAWord = x;
    }

    IEnumerator IdleSearchBarAnim()
    {
        SearchBarText.text = " |";
        textAnim.SetCharacterPerSecond(2);
        WaitingSearchBarText.text = initWaitingSearchBarText;

        while (isWaitingAWord)
        {
            textAnim.AnimateTyping();
            yield return new WaitForSeconds(1f);
        }
    }

    public void ResertBar(Component seder, object obj)
    {
        StopIdleSearchBarAnim();
        isWaitingAWord = false;
        ClearWord();
    }
}
