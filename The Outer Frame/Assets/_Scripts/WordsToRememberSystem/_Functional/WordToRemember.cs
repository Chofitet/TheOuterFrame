using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordToRemember : MonoBehaviour
{
    WordData word;
    [SerializeField] TMP_Text textField;

    public void Initialize(WordData _word)
    {
        word = _word;
        textField.text = _word.GetName();
    }
}
