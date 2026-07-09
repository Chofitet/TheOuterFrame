using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookPage : MonoBehaviour
{
    [SerializeField] Transform WordAnchors;
    [SerializeField] int PageNum;
    [SerializeField] GameObject WordPrefab;
    int numWords;
    List<Transform> WordSpots = new List<Transform>();
    bool isFull;
    [SerializeField]int PassToSecondColumn;

    private void Start()
    {
        for (int i = 0; i < WordAnchors.childCount; i++)
        {
            WordSpots.Add(WordAnchors.GetChild(i));
        }
    }


    public void DisableEnable(bool x)
    {
        gameObject.GetComponent<Canvas>().enabled = x;
    }

    public List<Transform> GetWordSpots() { return WordSpots; }
    public int GetNumOfWords() { return numWords; }

    public bool GetIsFull() { return isFull; }

    public GameObject InstanciateWord()
    {
        GameObject wordaux = Instantiate(WordPrefab, WordSpots[numWords].position, WordSpots[numWords].rotation, transform);
        numWords++;
        if (PassToSecondColumn < numWords) wordaux.GetComponent<NotebookWordInstance>().IsInSecondColumn();
        if (numWords == WordAnchors.childCount) isFull = true;
        return wordaux;
    }
    
}


