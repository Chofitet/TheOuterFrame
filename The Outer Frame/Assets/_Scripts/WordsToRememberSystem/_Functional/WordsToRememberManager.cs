using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsToRememberManager : MonoBehaviour
{
    [SerializeField] List<WordData> WordsToRemember;
    [SerializeField] GameEvent OnAddWordsToRemember;

    private void Start()
    {
        OnAddWordsToRemember?.Invoke(this, WordsToRemember);
    }

}
