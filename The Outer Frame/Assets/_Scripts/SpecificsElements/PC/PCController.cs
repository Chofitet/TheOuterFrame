using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class PCController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameEvent OnPCSearchWord;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] TMP_Text WikiTitleSearchedWord;
    [SerializeField] GameEvent OnWikiWindow;
    [SerializeField] GameEvent OnWordAccessScreen;
    [SerializeField] GameEvent OnKeyBoardSound;
    [SerializeField] List<GameObject> PanelsAppearsOnSearch = new List<GameObject>();
    GameEvent LastWindow;
    bool isWaitingAWord;
    bool inWordAccessWindow;

    WordData word;
    bool isInPCView;
    TypingAnimText textAnim;

    private void Start()
    {
        isWaitingAWord = true;
        textAnim = SearchBar.GetComponent<TypingAnimText>();
        textAnim.SetCharacterPerSecond(2);
        foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(false);
        StartCoroutine(IdleSearchBarAnim());
    }

    //OnSelectedWordInNotebook
    public void CompleteSeachBar(Component sender, object obj)
    {
        if (!isInPCView) return;
        if (inWordAccessWindow) return;
        WordData _word = (WordData)obj;
        
        word = _word;
        SearchBar.text = word.GetForm_DatabaseNameVersion();
        StopCoroutine(IdleSearchBarAnim());
        textAnim.SetCharacterPerSecond();
        isWaitingAWord = false;
        SearchBar.GetComponent<TypingAnimText>().AnimateTyping();
        OnKeyBoardSound?.Invoke(this, null);

        

    }

    string DeleteSpetialCharacter(string txt)
    {
        return Regex.Replace(txt, @"[\?\.,\n\r]", "");
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

    public void SearchHyperLink(Component sender, object obj)
    {
        if (!isInPCView) return;
        WordData _word = (WordData)obj;
        word = _word;
        SearchWordInWiki();
    }

    public void UpdateDataBase(Component sender, object obj)
    {
        SearchWordInWiki();
        LastWindow?.Invoke(this, null);
    }

    public void SearchWordInWiki()
    {
        if (!word)
        {
            SearchBar.text = " |";
            if(isInPCView) OnShakeNotebook?.Invoke(this, null);
            WikiTitleSearchedWord.text = "";
            foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(false);
            OnPCSearchWord?.Invoke(this, word);
            return;
        }

        foreach (GameObject g in PanelsAppearsOnSearch) g.SetActive(true);
        DataBaseType db = WordsManager.WM.RequestBDWikiData(word);

        if (db.GetAccessWord() && !db.GetisWordAccessFound())
        {
            OnWordAccessScreen?.Invoke(this, word);
            inWordAccessWindow = true;
            return;
        }

        WikiTitleSearchedWord.text = word.GetForm_DatabaseNameVersion();

        isWaitingAWord = true;
        StopAllCoroutines();
        StartCoroutine(IdleSearchBarAnim());
        
        OnPCSearchWord?.Invoke(this, word);

        word = null;
    }

    public void CloseWordAccessWindow(Component sender, object obj)
    {
        inWordAccessWindow = false;
    }

    public void ChangeWindow(GameEvent gameEvent)
    {
        gameEvent?.Invoke(this, null);
        LastWindow = gameEvent;
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
