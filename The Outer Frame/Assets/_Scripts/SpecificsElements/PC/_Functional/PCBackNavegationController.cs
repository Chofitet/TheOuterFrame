using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class PCBackNavegationController : MonoBehaviour
{
    [SerializeField] Button BackBTN;
    [SerializeField] Button FrontBTN;
    [SerializeField] Button BackToDB;
    [SerializeField] GameEvent BackLastEntry;
    [SerializeField] GameEvent OnWikiWindow;
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
            OnSearchWindow?.Invoke(this, null);
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

            BackBTN.interactable = index >= 0;

            FrontBTN.interactable = index < SearchedWordsHistory.Count - 1;

        }
        else
        {
            Back.SetActive(true);
            Front.SetActive(true);
            BackDB.SetActive(false);
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

}
