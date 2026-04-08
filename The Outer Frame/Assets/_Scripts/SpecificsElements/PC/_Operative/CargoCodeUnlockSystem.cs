using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class CargoCodeUnlockSystem : MonoBehaviour
{
    [SerializeField] GameObject SymbolsSlotsContainer;
    CargoCodeDigitController[] symbolsSlots;
   
    [SerializeField] GameObject Conteiner;
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameEvent OnCloseWoredAccessWindow;
    [SerializeField] Image SearchBarGameObject;
    [SerializeField] GameEvent OnKeyboardSoundEvent;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] Button SearchButton;
    [SerializeField] GameEvent OnDisableSearchBar;
    [SerializeField] WordData AccessCode;
    [SerializeField] DataBaseType dataBaseCargo;
    [SerializeField] GameObject failButton;

    [SerializeField] GameObject codePanelConteiner;
    [SerializeField] GameObject CargoOpenText;
    [SerializeField] GameObject CargoOpenButton;

    WordData TryAccessWord;
    private bool isInPCView;
    bool isUnlockingPage;

    TypingAnimText textAnim;
    bool isWaitingAWord;

    Vector2 originalOffseMin;
    void Start()
    {
        symbolsSlots = SymbolsSlotsContainer.GetComponentsInChildren<CargoCodeDigitController>();
        textAnim = SearchBar.GetComponent<TypingAnimText>();

        Vector2 offsetMin = textAnim.gameObject.GetComponent<RectTransform>().offsetMin;
        offsetMin.x = 0;
        textAnim.gameObject.GetComponent<RectTransform>().offsetMin = offsetMin;
    }

    public void OpenPanel(Component sender, object obj)
    {
        OnDisableSearchBar?.Invoke(this, true);
        Conteiner.SetActive(true);
        startVerticalBarAnim();
    }

    public void TryOpenCargo()
    {
        isUnlockingPage = true;
        if (CheckWordAndSymbolsAreCorrect())
        {
            SearchButton.interactable = false;

            WordsManager.WM.AddStateOnHistory(dataBaseCargo.GetwordToUnlock(), dataBaseCargo.GetUnlockState());
            WordsManager.WM.AddStateOnSeenHistory(dataBaseCargo.GetwordToUnlock(), dataBaseCargo.GetUnlockState());

            StartCoroutine(CargoDoorOpening());
        }
        else
        {
            StartCoroutine(CargoDooorFailOpening());
        }
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
        if (!Conteiner.activeSelf) return;

        Vector2 offsetMin = textAnim.gameObject.GetComponent<RectTransform>().offsetMin;
        offsetMin.x = 2.34f;
        textAnim.gameObject.GetComponent<RectTransform>().offsetMin = offsetMin;

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


    bool CheckWordAndSymbolsAreCorrect()
    {
        bool firstConditional = false;

        if(TryAccessWord == AccessCode) firstConditional = true;

        if(firstConditional && CheckSymbolsAreCorrect()) return true;
        else return false;
    }

    bool CheckSymbolsAreCorrect()
    { 
        foreach(CargoCodeDigitController symbol in symbolsSlots)
        {
            if(!symbol.GetIsInCorrectSymbol()) return false;
        }
        return true;
    }


    public void PressOnSearchBar()
    {
        if (SearchBar.text == " |")
        {
            OnShakeNotebook?.Invoke(null, null);
        }
    }

    public void ClosePanelBTN()
    {
        ClosePanel(null, null);
    }

    public void ClosePanel(Component sender, object obj)
    {
        if (isUnlockingPage) return;
        StopCoroutine(IdleSearchBarAnim());
        Conteiner.SetActive(false);
        TryAccessWord = null;
        OnCloseWoredAccessWindow?.Invoke(this, null);
        OnDisableSearchBar?.Invoke(this, false);
    }

    void startVerticalBarAnim()
    {
        if (!Conteiner.activeSelf) return;
        SearchButton.interactable = true;
        SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 1f);
        SearchBar.text = "";
        isWaitingAWord = true;

        Vector2 offsetMin = textAnim.gameObject.GetComponent<RectTransform>().offsetMin;
        offsetMin.x = 0;
        textAnim.gameObject.GetComponent<RectTransform>().offsetMin = offsetMin;

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

    IEnumerator CargoDoorOpening()
    {
        codePanelConteiner.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        CargoOpenText.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        CargoOpenText.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        CargoOpenText.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        CargoOpenText.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        CargoOpenText.SetActive(true);
        CargoOpenButton.SetActive(true);
        isUnlockingPage = false;
    }

    IEnumerator CargoDooorFailOpening()
    {
        foreach (CargoCodeDigitController digit in symbolsSlots) digit.InactiveBackPanel();
        SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 0.5f);
        failButton.SetActive(true);
        yield return new WaitForSeconds(1f);
        foreach (CargoCodeDigitController digit in symbolsSlots) digit.ActiveBackPanel();
        SearchBarGameObject.color = new Color(SearchBarGameObject.color.r, SearchBarGameObject.color.g, SearchBarGameObject.color.b, 1f);
        failButton.SetActive(false);
        isUnlockingPage = false;
        //startVerticalBarAnim();
    }

}
