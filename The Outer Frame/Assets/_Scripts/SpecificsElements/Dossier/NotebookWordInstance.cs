using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotebookWordInstance : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject strikethrough;
    WordData wordReference;

    public void Initialization(WordData word)
    {
        wordReference = word;
        text.text = wordReference.GetName();
        GetComponent<Button>().onClick.AddListener(SetSelectedWord);

        if (wordReference.GetInactiveState())
        {
            GetComponent<Button>().enabled = false;
            strikethrough.SetActive(true);
        }
    }

    void SetSelectedWord()
    {
        text.text = "<u>" + wordReference.GetName() + "</u>";
        WordSelectedInNotebook.Notebook.SetSelectedWord(wordReference);
    }

    public void ClearUnderline()
    {
        text.text = wordReference.GetName();
    }
}
