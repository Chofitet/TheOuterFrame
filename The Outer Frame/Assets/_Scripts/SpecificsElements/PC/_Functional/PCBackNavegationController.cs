using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PCBackNavegationController : MonoBehaviour
{
    [Header("DB Navegation Buttons")]
    [SerializeField] Button BackBTN;
    [SerializeField] Button FrontBTN;
    [SerializeField] Button BackToDB;
    [SerializeField] GameEvent BackLastEntry;
    [SerializeField] GameEvent OnWikiWindow;
    [SerializeField] GameEvent OnLogWindow;
    [SerializeField] GameEvent OnSearchWindow;
    List<WordData> SearchedWordsHistory = new List<WordData>();
    GameObject Back;
    GameObject Front;
    GameObject BackDB;
    bool isInWikiWindow = true;
    int index = -1;
    bool IsUsingBackFrontBTN;
    [SerializeField] WordData Irrelevant;

    private void OnEnable()
    {
        Back = BackBTN.gameObject;
        Front = FrontBTN.gameObject;
        BackDB = BackToDB.gameObject;
        ButtonsAppear();
    }

    //OnSearchWord
    public void UpdateSearchedHistory(Component sender, object obj)
    {
        if (IsUsingBackFrontBTN)
        {
            IsUsingBackFrontBTN = false;
            return;
        }

        WordData word = (WordData)obj;




        if (SearchedWordsHistory.Count != 0)
        {
            if (SearchedWordsHistory.Last() == null || SearchedWordsHistory.Last() == Irrelevant) SearchedWordsHistory.RemoveAt(SearchedWordsHistory.Count - 1);


            if (SearchedWordsHistory.Count != 0) if (word == SearchedWordsHistory.Last()) return;
        }



        SearchedWordsHistory.Add(word);
        index = SearchedWordsHistory.Count - 1;
        // OnWikiWindow?.Invoke(this, null);
        ButtonsAppear();

    }


    public void BackInHistory()
    {
        if (!isInWikiWindow)
        {
            OnWikiWindow?.Invoke(this, null);
            IsUsingBackFrontBTN = false;
            ButtonsAppear();
            return;
        }

        if (index <= 0)
        {
            // OnSearchWindow?.Invoke(this, null); // Descomentar para que funcione el SearchWindow
            IsUsingBackFrontBTN = false;
            ButtonsAppear();
            return;
        }

        index--;

        WordData word = SearchedWordsHistory[index];

        IsUsingBackFrontBTN = true;
        BackLastEntry?.Invoke(this, word);

        ButtonsAppear();

    }

    public void GoFrontHistory()
    {
        if (!isInWikiWindow)
        {
            if (SearchedWordsHistory.Count == 0 ||
                SearchedWordsHistory[0] == null ||
                SearchedWordsHistory[0] == Irrelevant)
                return;

            index = 0;
            OnWikiWindow?.Invoke(this, null);

            IsUsingBackFrontBTN = true;
            BackLastEntry?.Invoke(this, SearchedWordsHistory[index]);

            ButtonsAppear();
            return;
        }
        else
        {
            OnWikiWindow?.Invoke(this, null);
        }

        // Si estamos en Wiki, avanzar normalmente.
        if (index >= SearchedWordsHistory.Count - 1)
            return;

        index++;

        WordData word = SearchedWordsHistory[index];

        IsUsingBackFrontBTN = true;
        BackLastEntry?.Invoke(this, word);

        ButtonsAppear();
    }

    void ButtonsAppear()
    {
        if (isInWikiWindow)
        {
            Back.SetActive(true);
            Front.SetActive(true);
            BackDB.SetActive(false);

            BackBTN.interactable = index >/*=*/ 0; // Descomentar para que funcione el SearchWindow

            FrontBTN.interactable = index < SearchedWordsHistory.Count - 1;

        }
        else
        {
            Back.SetActive(false);
            Front.SetActive(false);
            BackDB.SetActive(true);
        }

    }

    public void CheckWindowWiki(Component sender, object obj)
    {
        isInWikiWindow = true;

        ButtonsAppear();
    }

    public void CheckIsNotWindowWiki(Component sender, object obj)
    {
        isInWikiWindow = false;

        ButtonsAppear();
    }

    public void DeleteHistory(Component sender, object obj)
    {
        SearchedWordsHistory.Clear();
        Debug.Log("History clear");
    }

    public void SearchWindow(Component sender, object obj)
    {
        BackBTN.interactable = false;
        FrontBTN.interactable = true;
    }

    /// LOG and DB buttons

    [Header("LOG and DB buttons")]

    [SerializeField] Button DataBaseBTN;
    [SerializeField] Button EventLogBTN;

    [SerializeField] GameEvent OnPressBackToLogBTN;
    [SerializeField] GameEvent OnPressToLogBTN;
    [SerializeField] PCController PCController;

    [SerializeField] GameEvent OnRefreshLogScreen;
    [SerializeField] GameEvent OnRefreshDBScreen;

    [SerializeField] Sprite hoverUnpressed;
    [SerializeField] Sprite hoverSelected;
    [SerializeField] Sprite Unpressed;
    [SerializeField] Sprite Selected;

    bool isOnWikiWindow = true;
    bool isOnLogWindow;

    public void PressDBButton()
    {
        if (!isOnWikiWindow)
        {
            PCController.ChangeWindow(OnWikiWindow);
            OnPressBackToLogBTN?.Invoke(this, null);

            ActivateDBButton();
        }
        else
        {
            OnRefreshDBScreen?.Invoke(this, null);
        }
    }

    public void PressLogButton()
    {
        if (!isOnLogWindow)
        {
            PCController.ChangeWindow(OnLogWindow);
            OnPressToLogBTN?.Invoke(this, null);

            ActivateLogButton();
        }
        else
        {
            OnRefreshLogScreen.Invoke(this, null);
        }
    }

    void ActivateDBButton()
    {
        DataBaseBTN.image.sprite = Selected;
        EventLogBTN.image.sprite = Unpressed;

        SpriteState StateSelected = EventLogBTN.spriteState;
        StateSelected.highlightedSprite = hoverSelected;
        DataBaseBTN.spriteState = StateSelected;

        SpriteState StateUnselected = DataBaseBTN.spriteState;
        StateUnselected.highlightedSprite = hoverUnpressed;
        EventLogBTN.spriteState = StateUnselected;

        isOnWikiWindow = true;
        isOnLogWindow = false;

    }

    void ActivateLogButton()
    {
        EventLogBTN.image.sprite = Selected;
        DataBaseBTN.image.sprite = Unpressed;

        SpriteState StateSelected = EventLogBTN.spriteState;
        StateSelected.highlightedSprite = hoverSelected;
        EventLogBTN.spriteState = StateSelected;

        SpriteState StateUnselected = DataBaseBTN.spriteState;
        StateUnselected.highlightedSprite = hoverUnpressed;
        DataBaseBTN.spriteState = StateUnselected;


        isOnLogWindow = true;
        isOnWikiWindow = false;
    }

    public void ActivateDBButton(Component sender, object obj)
    {
        ActivateDBButton();
    }

    public void ActivateLogButton(Component sender, object obj)
    {
        ActivateLogButton();
    }
}
