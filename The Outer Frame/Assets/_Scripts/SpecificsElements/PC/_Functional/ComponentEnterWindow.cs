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
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] SliderUpdateAnim Slider;
    [SerializeField] GameObject SearchButton;
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
        if (x)
        {
            SearchBar.text = " |";
            startVerticalBarAnim();
        }
        else StopAllCoroutines();
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
        if (!content.activeSelf) return;
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
        if (SearchBar.text == " |")
        {
            OnShakeNotebook?.Invoke(this, null);
        }
        else
        {
            Invoke("UnlockPage", 2f);
            isUnlockingPage = true;
            SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 0.5f);
            Slider.gameObject.SetActive(true);
            Slider.AnimSlider(this, null);
            SearchButton.GetComponent<Button>().interactable = false;

            if (TryAccessWord == ComponentWord)
            {
                OnGooIdentificatorComponentAccessGranted?.Invoke(this, true);
            }
            else
            {
                OnGooIdentificatorComponentAccessGranted?.Invoke(this, false);

            }
        }
        if (TryAccessWord == null)
        {
            SearchBar.text = " |";
        }
    }

    public void PressSearchbar()
    {
        if (SearchBar.text == " |")
        {
            OnShakeNotebook?.Invoke(this, null);
        }
    }

    public void Reset()
    {
        SearchBar.text = " |";
        TryAccessWord = null;
        isUnlockingPage = false;
        SearchButton.GetComponent<Button>().interactable = true;
        Slider.gameObject.SetActive(false);
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
