using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentEnterWindow : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameObject content;
    [SerializeField] GameEvent OnKeyboardSoundEvent;
    [SerializeField] Image SearchBarGameObject;
    [SerializeField] WordData ComponentWord;
    [SerializeField] GameEvent OnGooIdentificatorComponentAccessGranted;
    WordData SearchedWord;
    WordData TryAccessWord;
    private bool isInPCView = true;
    bool isUnlockingPage;

    bool isCodeRight;

    TypingAnimText textAnim;
    bool isWaitingAWord;
    private void Start()
    {
        textAnim = SearchBar.GetComponent<TypingAnimText>();
    }

    public void ShowPanel(bool x)
    {
        content.SetActive(x);
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
        if (isUnlockingPage) return;
        SearchBar.text = " |";
        if (!isInPCView) return;
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
        if (TryAccessWord == ComponentWord)
        {
            Invoke("UnlockPage", 2f);
            isUnlockingPage = true;
            SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 0.5f);
            SearchBar.text = "ACCESS GRANTED";
            OnGooIdentificatorComponentAccessGranted?.Invoke(this, null);
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

    public void Reset()
    {
        SearchBar.text = " |";
        TryAccessWord = null;
        isUnlockingPage = false;
        SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 1f);
        content.SetActive(false);
    }


    void UnlockPage()
    {
        isUnlockingPage = false;
        //content.SetActive(false);
    }

    void startVerticalBarAnim()
    {
        isWaitingAWord = true;
        textAnim.SetCharacterPerSecond(2);
        StartCoroutine(IdleSearchBarAnim());
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
