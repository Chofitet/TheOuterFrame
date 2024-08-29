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

        if (!word.GetIsFound())
        {
            word.SetIsFound();
            text.gameObject.GetComponent<FadeWordsEffect>().StartEffect();
        }
        
    }

    public void ReplaceWord(WordData word)
    {
        text.text = wordReference.GetName();
        StartCoroutine(AnimFade(text,false,text,true,word.GetName()));
        wordReference = word;
        word.SetIsFound();
    }

    IEnumerator AnimFade(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        first.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent1);
        yield return new WaitForSeconds(0.5f);
        if (first == second) first.text = txt;
        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
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

    public WordData GetWord() { return wordReference; }
}
