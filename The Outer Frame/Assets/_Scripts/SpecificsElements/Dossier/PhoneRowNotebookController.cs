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

    public void Initialization(WordData _word)
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ButtonPress);
        button.enabled = false;
        word = _word;
        txtName.text = word.GetName();

        if(word.GetIsAPhoneNumber())
        {
            txtName.text = "?????";
            Num.text = word.GetPhoneNumber();
        }

        if(word.GetIsPhoneNumberFound())
        {
            Num.text = word.GetPhoneNumber();
            button.enabled = true;
        }
    }

    private void ButtonPress()
    {
        txtName.text = "<u>" + word.GetName() + "</u>";
        
        if (word.GetIsAPhoneNumber())
        {
            txtName.text = "?????";
        }
        WordSelectedInNotebook.Notebook.SetSelectedWord(word);
    }

    public void ClearUnderline()
    {
        
        txtName.text = word.GetName();
        if (word.GetIsAPhoneNumber())
        {
            txtName.text = "?????";
        }
    }
}
