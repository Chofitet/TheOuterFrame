using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PCBackNavegationController : MonoBehaviour
{
    [SerializeField] Button BackBTN;
    [SerializeField] Button FrontBTN;
    [SerializeField] Button BackToDB;
    [SerializeField] GameEvent BackLastEntry;
    [SerializeField] GameEvent OnWikiWindow;
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


            if(SearchedWordsHistory.Count != 0) if (word == SearchedWordsHistory.Last()) return;
        }



        SearchedWordsHistory.Add(word);
        index = SearchedWordsHistory.Count - 1;
        
        ButtonsAppear();

    }

  

    public void BackInHistory()
    {
        if(!isInWikiWindow)
        {
            OnWikiWindow?.Invoke(this, null);
            IsUsingBackFrontBTN = false;
            return;
        }

        if (SearchedWordsHistory.Last() == null || SearchedWordsHistory.Last() == Irrelevant)
        {
            SearchedWordsHistory.RemoveAt(SearchedWordsHistory.Count - 1);
        }

        index--;
        WordData word = SearchedWordsHistory[index];
        IsUsingBackFrontBTN = true;

        BackLastEntry?.Invoke(this, word);
        ButtonsAppear();

    }

    public void GoFrontHistory()
    {
        index = index + 1;
        WordData word = SearchedWordsHistory[index];
        IsUsingBackFrontBTN = true;
        BackLastEntry?.Invoke(this, word);
        ButtonsAppear();
    }

    void ButtonsAppear()
    {
        if(isInWikiWindow)
        {
            Back.SetActive(true);
            Front.SetActive(true);
            BackDB.SetActive(false);

            if (index <= 0) BackBTN.interactable = false;
            else BackBTN.interactable = true; 

            if (index >= SearchedWordsHistory.Count - 1) FrontBTN.interactable = false;
            else FrontBTN.interactable = true;
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

}
