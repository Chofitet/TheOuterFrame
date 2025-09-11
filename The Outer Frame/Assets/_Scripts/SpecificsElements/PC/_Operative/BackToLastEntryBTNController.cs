using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToLastEntryBTNController : MonoBehaviour
{
    WordData word;
    [SerializeField] GameEvent OnBackToLastEntry;
    public void SetWordToBack(WordData obj)
    {
        word = obj;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void BackToLastEntry()
    {
        OnBackToLastEntry?.Invoke(this, word);
    }
}
