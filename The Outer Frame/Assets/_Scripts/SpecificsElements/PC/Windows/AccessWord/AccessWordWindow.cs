using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessWordWindow : MonoBehaviour
{
    [SerializeField] GameEvent OnPCSearchWord;
    [SerializeField] GameObject Conteiner;
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameEvent OnCloseWoredAccessWindow;
    [SerializeField] GameEvent OnAccessWiki;
    [SerializeField] Image SearchBarGameObject;
    [SerializeField] GameEvent OnKeyboardSoundEvent;
    WordData SearchedWord;
    WordData TryAccessWord;
    private bool isInPCView;
    bool isUnlockingPage;

    TypingAnimText textAnim;
    bool isWaitingAWord;
    private void Start()
    {
        textAnim = SearchBar.GetComponent<TypingAnimText>();
    }

    //OnWordAccessScreen
    public void SetSearchedWord(Component sender, object obj)
    {
        WordData S_word = (WordData)obj;

        SearchedWord = S_word;
        Conteiner.SetActive(true);
        startVerticalBarAnim();
        //AccessWord = WordsManager.WM.RequestBDWikiData(S_word).GetAccessWord();
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

    //OnSelectedWordInNotebook
    public void CompleteAccessBar(Component sender, object obj)
    {
        SearchBar.text = " |";
        if (!isInPCView) return;
        if (!Conteiner.activeSelf) return;
        WordData _word = (WordData)obj;
        TryAccessWord = _word;
        SearchBar.text = DeleteSpetialCharacter(TryAccessWord.GetName());
        StopCoroutine(IdleSearchBarAnim());
        textAnim.SetCharacterPerSecond();
        isWaitingAWord = false;
        SearchBar.GetComponent<TypingAnimText>().AnimateTyping();
        OnKeyboardSoundEvent?.Invoke(this, null);
    }

    string DeleteSpetialCharacter(string txt)
    {
        return Regex.Replace(txt, @"[\?\.,\n\r]", "");
    }

    public void TryAccess()
    {
        if (!Conteiner.activeSelf) return;
        if (TryAccessWord == WordsManager.WM.RequestBDWikiData(SearchedWord).GetAccessWord())
        {
            Invoke("UnlockPage", 2f);
            isUnlockingPage = true;
            SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 0.5f);
            SearchBar.text = "ACCESS GRANTED";
            OnAccessWiki?.Invoke(this, null);

            WordsManager.WM.RequestBDWikiData(SearchedWord).SetisWordAccessFound();
        }
        else
        {
            SearchBar.text = "ACCESS DENIED";
        }

        if (TryAccessWord == null)
        {
            SearchBar.text = " |";
        }
    }

    void UnlockPage()
    {
        isUnlockingPage = false;
        OnPCSearchWord?.Invoke(this, SearchedWord);
        ClosePanel(null,null);
    }

    public void ClosePanelBTN()
    {
        ClosePanel(null, null);
    }

    public void ClosePanel(Component sender,object obj)
    {
        if (isUnlockingPage) return;
        StopCoroutine(IdleSearchBarAnim());
        Conteiner.SetActive(false);
        SearchedWord = null;
        TryAccessWord = null;
        OnCloseWoredAccessWindow?.Invoke(this, null);
    }

    void startVerticalBarAnim()
    {
        if (!Conteiner.activeSelf) return;
        isWaitingAWord = true;
        textAnim.SetCharacterPerSecond(2);
        StartCoroutine(IdleSearchBarAnim());
    }

    IEnumerator IdleSearchBarAnim()
    {
        SearchBar.text = " |";
        textAnim.SetCharacterPerSecond(2);

        while (isWaitingAWord && Conteiner.activeSelf)
        {
            textAnim.AnimateTyping();
            yield return new WaitForSeconds(1f);
        }
    }

}
