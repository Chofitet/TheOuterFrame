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
    WordData word;
    Button button;

    public void Initialization(WordData _word)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonPress);
        word = _word;
        txtName.text = word.GetName();

        if (word.GetIsAPhoneNumber())
        {
            txtName.text = "?????";
            Num.text = word.GetPhoneNumber();
            button.enabled = true;
            StartCoroutine(AnimFade(Num, true , txtName,true));
        }

        if (!word.GetIsPhoneNumberFound())
        { 
            StartCoroutine(AnimFade(txtName, true, Num, true));
        }
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
    }

    public WordData GetWord() { return word; }
}
