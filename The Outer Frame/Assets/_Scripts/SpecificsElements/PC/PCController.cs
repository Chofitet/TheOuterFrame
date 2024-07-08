using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PCController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] TMP_Text wikiData;
    [SerializeField] GameEvent OnPCSearchWord;

    WordData word;

    public void CompleteSeachBar()
    {
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();
        SearchBar.text = word.GetName();
    }

    public void SearchWordInWiki()
    {
        OnPCSearchWord?.Invoke(this, word);
    }

    public void ChangeWindow(GameEvent gameEvent)
    {
        gameEvent?.Invoke(this, null);
    }

    



}
