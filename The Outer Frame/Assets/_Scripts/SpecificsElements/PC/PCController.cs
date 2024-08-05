using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PCController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameEvent OnPCSearchWord;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] GameObject DataBaseUpdatedWindow;
    [SerializeField] GameEvent OnWikiWindow;
    bool isWaitingAWord;

    WordData word;
    bool isInPCView;
    TypingAnimText textAnim;

    private void Start()
    {
        isWaitingAWord = true;
        textAnim = SearchBar.GetComponent<TypingAnimText>();
        textAnim.SetCharacterPerSecond(2);
        StartCoroutine(IdleSearchBarAnim());
    }

    //OnSelectedWordInNotebook
    public void CompleteSeachBar(Component sender, object obj)
    {
        if (!isInPCView) return;
        WordData _word = (WordData)obj;
        word = _word;
        SearchBar.text = word.GetName();
        StopCoroutine(IdleSearchBarAnim());
        textAnim.SetCharacterPerSecond();
        isWaitingAWord = false;
        SearchBar.GetComponent<TypingAnimText>().AnimateTyping();
        
    }

    //OnChangeView
    public void GetActualView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.PCView)
        {
            isInPCView = true;
        }
        else isInPCView = false;
    }

    public void SearchWordInWiki()
    {
        if (!word)
        {
            SearchBar.text = "Insert a word";
            OnShakeNotebook?.Invoke(this, null);
            return;
        }

        isWaitingAWord = true;
        StopAllCoroutines();
        StartCoroutine(IdleSearchBarAnim());
        
        OnPCSearchWord?.Invoke(this, word);
    }

    public void ChangeWindow(GameEvent gameEvent)
    {
        gameEvent?.Invoke(this, null);
    }

    public void UpdatePC(Component sender, object obj)
    {
        DataBaseUpdatedWindow.SetActive(true);
        Invoke("DesactiveDataBaseUpdatedWindow", 2);
    }

    void DesactiveDataBaseUpdatedWindow()
    {
        DataBaseUpdatedWindow.SetActive(false);
        ChangeWindow(OnWikiWindow);
    }

    IEnumerator IdleSearchBarAnim()
    {
        SearchBar.text = " |";
        textAnim.SetCharacterPerSecond(2);

        while (isWaitingAWord)
        {
            textAnim.AnimateTyping();
            yield return new WaitForSeconds(1f);
        }
    }

    

}
