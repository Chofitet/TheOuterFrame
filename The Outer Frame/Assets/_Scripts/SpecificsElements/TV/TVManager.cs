using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TVManager : MonoBehaviour
{
    [SerializeField] TMP_Text NewsText;

    private void Start()
    {
        WordsManager.WM.OnChangeStateOfWord += RegisterChangeState;
    }

    private void OnDestroy()
    {
        WordsManager.WM.OnChangeStateOfWord -= RegisterChangeState;
    }

    void RegisterChangeState(string word)
    {
        NewsText.text = WordsManager.WM.RequestNew(word);
    }

}
