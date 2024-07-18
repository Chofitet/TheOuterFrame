using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WordSelectedInNotebook : MonoBehaviour
{
    public static WordSelectedInNotebook Notebook { get; private set; }
    [SerializeField] GameEvent OnSelectedWordInNotebook;
    WordData SelectedWord;

    List<WordData> WordsFound = new List<WordData>();
    [SerializeField] List<WordData> WordsfromBeginning = new List<WordData>();

    private void Awake()
    {

        if (Notebook != null && Notebook != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Notebook = this;
        }

    }

    private void Start()
    {
        foreach (WordData w in WordsfromBeginning)
        {
            AddWordToNotebook(null,w);
        }
    }

    public void AddWordToNotebook(Component sender, object obj)
    {
        WordData word = (WordData)obj;
        if (!word) return;

        if(word.GetIsAPhoneNumber())
        {
            SetFindedNumber(word);
            return;
        }

        WordsFound.Add(word);
        word.SetIsFound();
    }

    private void SetFindedNumber(WordData num)
    {
        bool isTheWordOfTheNumInList = false;
        foreach (WordData word in WordsFound)
        {
            if(word.GetPhoneNumber() == num.GetName())
            {
                word.SetIsPhoneNumberFound();
                isTheWordOfTheNumInList = true;
            }
        }

        if(!isTheWordOfTheNumInList)
        {
            WordsFound.Add(num);
        }
    }

    public void UnselectWord() => SelectedWord = null;


    public void SetSelectedWord(WordData word)
    {
        SelectedWord = word;
        OnSelectedWordInNotebook?.Invoke(this, SelectedWord);
    }

    public WordData GetSelectedWord(){return SelectedWord;}

    public List<WordData> GetWordsList() { return WordsFound; }

}

