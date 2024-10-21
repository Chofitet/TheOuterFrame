using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PCBackNavegationController : MonoBehaviour
{
    [SerializeField] Button BackBTN;
    [SerializeField] Button FrontBTN;
    [SerializeField] GameEvent BackLastEntry;
    [SerializeField] GameEvent OnWikiWindow;
    List<WordData> SearchedWordsHistory = new List<WordData>();
    GameObject Back;
    GameObject Front;
    bool isInWikiWindow = true;
    int index = -1;
    bool IsUsingBackFrontBTN;
    [SerializeField] WordData Irrelevant;
    

    private void OnEnable()
    {
        Back = BackBTN.gameObject;
        Front = FrontBTN.gameObject;
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


            if (word == SearchedWordsHistory.Last()) return;
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
            if (index <= 0) Back.SetActive(false);
            else Back.SetActive(true);

            if (index >= SearchedWordsHistory.Count - 1)  Front.SetActive(false);
            else Front.SetActive(true);
        }
        else
        {
            Back.SetActive(true);
            Front.SetActive(false);
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
