using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class NotebookWordInstance : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject strikethrough;
    [SerializeField] GameEvent OnWritingShakeNotebook;
    [SerializeField] GameEvent OnCrossWordSound;
    [SerializeField] GameEvent OnWritingNotebookSound;
    WordData wordReference;
    bool isCross;

    public void Initialization(WordData word)
    {
        wordReference = word;
        text.text = wordReference.GetName();
        GetComponent<Button>().onClick.AddListener(SetSelectedWord);

        float writingTime = 0;

        if (!word.GetIsFound())
        {
            writingTime = 0.5f;
            word.SetIsFound();
            text.gameObject.GetComponent<FadeWordsEffect>().StartEffect();
        }


        OnWritingShakeNotebook?.Invoke(this, writingTime);
        OnWritingNotebookSound?.Invoke(this, null);
        Invoke("Alpha1", 1);

    }

    public void EraseAnim()
    {
        text.gameObject.GetComponent<FadeWordsEffect>().StartEffect(false);
    }

    public void RefreshWord(Component sender, object obj)
    {
        if (wordReference.GetInactiveState() && !wordReference.GetEraseState())
        {
            GetComponent<Button>().enabled = false;
            strikethrough.SetActive(true);
            CrossOutWord();
        }
        
    }
    public void ReplaceWord(WordData word)
    {
        Debug.Log(wordReference.GetName());
        text.text = wordReference.GetName();
        if (isCross) EraseCrossWord();
        StartCoroutine(AnimFade(text,false,text,true,word.GetName()));
        wordReference = word;
        word.SetIsFound();
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
    }

    public void ReplaceWordInstantly(WordData word)
    {
        wordReference = word;
        word.SetIsFound();
        text.text = wordReference.GetName();
    }

    public void CrossOutWord()
    {
        if (isCross) return;
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.DOSizeDelta(new Vector2(text.GetComponent<RectTransform>().sizeDelta.x, line.sizeDelta.y),0.3f);
        OnCrossWordSound?.Invoke(this, null);
        isCross = true;
    }


    public void EraseCrossWord()
    {
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.pivot = new Vector2(1, 0);
        line.localPosition = new Vector2(line.sizeDelta.x, line.localPosition.y);
        line.DOSizeDelta(new Vector2(0, line.sizeDelta.y), 0.3f);
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

    void Alpha1()
    {
        text.color = new Vector4(text.color.r, text.color.g, text.color.b, 1);
    }

    public WordData GetWord() { return wordReference; }
}
