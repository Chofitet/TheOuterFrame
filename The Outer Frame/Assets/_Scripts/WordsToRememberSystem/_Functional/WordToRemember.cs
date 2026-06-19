using System;
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
    [SerializeField] List<MemberWordsModels> models = new List<MemberWordsModels>();
    bool isTaken;
    int ChosenPaper;

    public void Initialize(WordData _word, List<int> ChosenPapersList)
    {
        word = _word;
        textField.text = _word.GetName();

        int index = 0;
        foreach (MemberWordsModels paperModelData in models)
        {
            index ++;
            GameObject paper = paperModelData.GetModel(word.GetName().Length);

            if(paper)
            {
                if (ChosenPapersList.Contains(index)) continue;
                paper.SetActive(true);
                ChosenPaper = index;
                return;
            }
            
        }
    }

    public int GetChosenPaper() { return ChosenPaper; }

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

    public void SetWordModel(int CharactersNum)
    {

    }

    public WordData GetWord() { return word; }
}

[Serializable]
public class MemberWordsModels
{
    public GameObject model;
    public int MaxWordLenght;
    int PaperNum;

    public GameObject GetModel(int NumCharacters)
    {
        if(NumCharacters > MaxWordLenght) return model;

        return null;
    }

}