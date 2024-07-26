using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PCController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameEvent OnPCSearchWord;
    [SerializeField] GameObject DataBaseUpdatedWindow;
    [SerializeField] GameEvent OnWikiWindow;

    WordData word;
    bool isInPCView;

    //OnSelectedWordInNotebook
    public void CompleteSeachBar(Component sender, object obj)
    {
        if (!isInPCView) return;
        WordData _word = (WordData)obj;
        word = _word;
        SearchBar.text = word.GetName();
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
            SearchBar.text = "Enter word";
            return;
        }
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

}
