using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordSelectedInNotebook : MonoBehaviour
{
    public static WordSelectedInNotebook Notebook { get; private set; }
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
        WordsFound.Add(word);
        word.SetIsFound();
    }
    public void UnselectWord() => SelectedWord = null;


    public void SetSelectedWord(WordData word) => SelectedWord = word;
    

    public WordData GetSelectedWord(){return SelectedWord;}

    public List<WordData> GetWordsList() { return WordsFound; }

}

