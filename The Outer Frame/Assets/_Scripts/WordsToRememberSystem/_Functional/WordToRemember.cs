using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordToRemember : MonoBehaviour
{
    WordData word;
    [SerializeField] TMP_Text textField;
    [SerializeField] GameEvent OnAddMemberWord;
    [SerializeField] GameEvent OnRemoveMemberWord;

    bool isTaken;

    public void Initialize(WordData _word)
    {
        word = _word;
        textField.text = _word.GetName();
    }

    public void AddWordToMemberList(Component sender,object obj)
    {
        if ((GameObject)obj != gameObject) return;
        if (!isTaken)
        {
            OnAddMemberWord?.Invoke(this, gameObject);
            isTaken = true;
        }
        else
        {
            OnRemoveMemberWord?.Invoke(this, gameObject);
            isTaken = false;
        }


    }

    public WordData GetWord() { return word; }
}
