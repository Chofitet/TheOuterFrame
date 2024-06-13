using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordSelectedInNotebook : MonoBehaviour
{
   
    string SelectedWord;

    public static WordSelectedInNotebook Notebook { get; private set; }
    private List<string> CleanwordsTexts = new List<string>();
    private Dictionary<int, GameObject> wordsTexts = new Dictionary<int, GameObject>();

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

        SetDictionary();
    }

    void SetDictionary()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            wordsTexts.Add(i, child.gameObject);

            CleanwordsTexts.Add(child.name);
        }
    }

    public void SetSelectedWord(int Num)
    {
        SelectedWord = wordsTexts[Num].name;
        SetUnderline(Num);
    }

    public string GetSelectedWord()
    {
        return SelectedWord;
    }

    void SetUnderline(int Num)
    {
        TMP_Text auxText = wordsTexts[Num].transform.GetChild(0).GetComponent<TMP_Text>();
        CleanUnderline();
        auxText.text = "<u>" + auxText.text + "</u>";
    }

    void CleanUnderline()
    {
        for (int i = 0; i < wordsTexts.Count; i++)
        {
            TMP_Text auxText = wordsTexts[i].transform.GetChild(0).GetComponent<TMP_Text>();
            auxText.text = CleanwordsTexts[i];
        }
    }
}
