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
    [SerializeField] GameEvent OnBackToDefaultPosInVoid;
    [SerializeField] BoxCollider blockInput;
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
            StartCoroutine(BlockInput(0.3f));
        }
        else
        {
            OnBackToDefaultPosInVoid?.Invoke(this, null);
            OnRemoveMemberWord?.Invoke(this, gameObject);
            isTaken = false;
            if(isInBackView)
            StartCoroutine(BlockInput(1.3f));
            else StartCoroutine(BlockInput(0.3f));
        }
    }

    IEnumerator BlockInput(float time)
    {
        blockInput.enabled = true;
        yield return new WaitForSeconds(time);
        blockInput.enabled = false;
    }

    bool isInBackView;
    public void IsInBackView(Component sender, object obj)
    {
        isInBackView = true;
    }

    public void IsInDefaultView(Component sender, object obj)
    {
        isInBackView = false;
    }

    public WordData GetWord() { return word; }
}
