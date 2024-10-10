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
    [SerializeField] GameEvent OnSlidePhones;
    [SerializeField] GameEvent OnShowWordsNotebook;
    [SerializeField] GameEvent OnShowNumNotebook;
    [SerializeField] GameEvent OnDelayNotChangeView;
    [SerializeField] GameEvent OnButtonElementClick;
    [SerializeField] GameEvent OnRefreshNotebook;
    [SerializeField] GameEvent OnRemoveEraceInstance;
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

        if (!word.GetIsAPhoneNumber()) AddWord(word);
        else AddNumber(word);
        OnRefreshNotebook?.Invoke(this, null);
    }

    void AddWord(WordData word)
    {
        OnDelayNotChangeView?.Invoke(this, 1f);

        if (word.GetIsFound()) return;
        if (word.GetWordThatReplaces())
        {
            ReplaceWordInList(SearchForWordThatReplaceRetroactive(word), word);
        }
        else
        {
            WordsFound.Add(word);
        }

        OnSlidePhones?.Invoke(this, false);
        DeleteOtherWords(word);
        OnShowWordsNotebook?.Invoke(this, word);

        if(word.GetPhoneNumber() != "" )
        {
            StartCoroutine(SlideDelay(IsNumAlreadyInList(word), word));
            OnDelayNotChangeView?.Invoke(this, 1f);
        }

        
    }
    void AddNumber(WordData num)
    {
        if (!IsWordAlreadyExist(num))
        {
            WordsFound.Add(num);
            OnDelayNotChangeView?.Invoke(this, 2f);
        }
        else
        {
            IsWordAlreadyExist(num).SetIsPhoneNumberFound();
        }

        OnSlidePhones?.Invoke(this,true);
        OnShowNumNotebook?.Invoke(this, IsWordAlreadyExist(num));
    }

    IEnumerator SlideDelay(WordData num, WordData word)
    {
        yield return new WaitForSeconds(1.1f);
        OnSlidePhones?.Invoke(this, true);
        if(!num) OnShowNumNotebook?.Invoke(this, word);
        else OnShowNumNotebook?.Invoke(this, num);
        yield return new WaitForSeconds(1.1f);
        OnSlidePhones?.Invoke(this, false);
    }

    WordData IsNumAlreadyInList(WordData word)
    {
        WordData aux = null;
        foreach(WordData num in WordsFound)
        {
            if (!num.GetIsAPhoneNumber()) continue;
            if(num.GetName() == word.GetPhoneNumber())
            {
                aux = num;
                word.SetIsPhoneNumberFound();
                
            }
        }
        WordsFound.Remove(aux);
        return aux;
    }

    WordData IsWordAlreadyExist(WordData Num)
    {
        foreach(WordData word in WordsFound)
        {
            if(word.GetPhoneNumber() == Num.GetName())
            {
                return word;
            }
        }
        return null;
    }

    void ReplaceWordInList(WordData oldWord, WordData newWord)
    {
        int index = WordsFound.IndexOf(oldWord);
        if (index != -1)
        {
            WordsFound[index] = newWord;
            if(newWord.GetCopyHistory()) newWord.ReplaceHistory(oldWord);
        }
        else
        {
            oldWord.SetIsFound();
            WordsFound.Add(newWord);
        }
    }

    void DeleteOtherWords(WordData word)
    {
        foreach(WordData _word in WordsFound)
        {
            DeleteCrossoutWorsd DeleteInfo = word.CheckOtherWordDelete(_word);
            if (DeleteInfo == null) continue;
            else
            {
                if (DeleteInfo.isErase()) OnRemoveEraceInstance?.Invoke(this, _word);
                else _word.SetInactive();
            }

        }
    }


    WordData SearchForWordThatReplaceRetroactive(WordData word)
    {
        WordData currentWord = word.GetWordThatReplaces();
        WordData auxWord = word.GetWordThatReplaces();

        while (currentWord != null)
        {
            currentWord.SetIsFound();

            auxWord = currentWord;

            currentWord = currentWord.GetWordThatReplaces();

            if (currentWord == auxWord) break;
        }

        return auxWord;
    }

    public void UnselectWord() => SelectedWord = null;


    public void SetSelectedWord(WordData word)
    {
        SelectedWord = word;
        OnSelectedWordInNotebook?.Invoke(this, SelectedWord);
        if(actualView == ViewStates.OnTakenPaperView) OnButtonElementClick?.Invoke(this, ViewStates.DossierView);
    }

    ViewStates actualView;
    public void ActualView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
    }

    public WordData GetSelectedWord(){return SelectedWord;}

    public List<WordData> GetWordsList() { return WordsFound; }

}

