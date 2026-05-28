using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordToRemember : MonoBehaviour
{
    WordData word;
    [SerializeField] TMP_Text textField;
    [SerializeField] GameEvent OnAddMemberWord;

    public void Initialize(WordData _word)
    {
        word = _word;
        textField.text = _word.GetName();
       
    }

    public void AddWordToMemberList()
    {
        OnAddMemberWord?.Invoke(this, word);
    }
}
