using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneRowNotebookController : MonoBehaviour
{
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text Num;
    [SerializeField] GameEvent OnWritingShakeNotebook;
    [SerializeField] GameEvent OnWritingNotebookSound;
    WordData word;
    Button button;

    public void Initialization(WordData _word)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonPress);
        word = _word;
        txtName.text = word.GetName();
        float writingTime = 0;

        if (word.GetIsAPhoneNumber())
        {
            writingTime = 0.5f;
            txtName.text = "?????";
            Num.text = word.GetPhoneNumber();
            button.enabled = true;
            Num.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
            txtName.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
        }

        if (!word.GetIsPhoneNumberFound())
        {
            writingTime = 0.5f + 0.5f;
            Num.text = "?????";
            Num.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
            txtName.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
        }

        OnWritingShakeNotebook?.Invoke(this, writingTime);
        OnWritingNotebookSound?.Invoke(this, null);
    }

    public void UpdateNumber()
    {
        button.enabled = true;
        Num.text = "?????";
        StartCoroutine(AnimFade(Num, false, Num, true,word.GetPhoneNumber()));
    }

    public void ReplaceNumberWithWord(WordData _word)
    {
        word = _word;
        txtName.text = "?????";
        StartCoroutine(AnimFade(txtName, false, txtName, true,word.GetName()));
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
    }

    public void ReplaceNumber(WordData _word)
    {
        txtName.text = word.GetName();
        word = _word;
        StartCoroutine(AnimFade(txtName, false, txtName, true, _word.GetName()));
        
        if (_word.GetIsPhoneNumberFound())
        {
            string auxNum = "?????";
            auxNum = _word.GetPhoneNumber();

            StartCoroutine(AnimFade(Num, false, Num, true, auxNum));
        }
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
    }

    public void EraseAnim()
    {
        StartCoroutine(AnimFade(txtName, false, txtName, true, " "));

        StartCoroutine(AnimFade(Num, false, Num, true, " "));
    }

    private void ButtonPress()
    {
        if(word.GetIsAPhoneNumber())
        {
            Num.text = "<u>" + word.GetName() + "</u>";
            WordSelectedInNotebook.Notebook.SetSelectedWord(word);
            return;
        }

        txtName.text = "<u>" + word.GetName() + "</u>";
        
        WordSelectedInNotebook.Notebook.SetSelectedWord(word);
    }

    public void ClearUnderline()
    {
        if(word.GetIsAPhoneNumber())
        {
            Num.text = word.GetName();
            return;
        }
        txtName.text = word.GetName();
    }

    IEnumerator AnimFade(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        first.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent1);
        yield return new WaitForSeconds(0.5f);
        if(first == second) first.text = txt;
        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
        OnWritingNotebookSound?.Invoke(this, null);
    }

    public WordData GetWord() { return word; }
}
