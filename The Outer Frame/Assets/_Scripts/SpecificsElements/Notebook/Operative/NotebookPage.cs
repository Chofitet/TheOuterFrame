using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NotebookPage : MonoBehaviour
{
    [SerializeField] Transform WordAnchors;
    [SerializeField] GameObject WordPrefab;
    int numWords;
    List<Transform> WordSpots = new List<Transform>();
    bool isFull;
    [SerializeField] int PassToSecondColumn;
    [SerializeField] RectTransform rect;
    [SerializeField] float ShowWordsDuration;
    [SerializeField] float HideWordsDuration;

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

        /*if (x) AnimateMask(110, ShowWordsDuration,0.1f);
        else AnimateMask(0, HideWordsDuration,0);*/
    }

    public List<Transform> GetWordSpots() { return WordSpots; }
    public int GetNumOfWords() { return numWords; }

    public bool GetIsFull() { return isFull; }

    public GameObject InstanciateWord()
    {
        GameObject wordaux = Instantiate(WordPrefab, WordSpots[numWords].position, WordSpots[numWords].rotation, rect.gameObject.transform);
        numWords++;
        if (PassToSecondColumn < numWords) wordaux.GetComponent<NotebookWordInstance>().IsInSecondColumn();
        if (numWords == WordAnchors.childCount) isFull = true;
        return wordaux;
    }

    public Sequence AnimateMask(float targetWidth, float duration, float startingDelay)
    {
        return DOTween.Sequence().PrependInterval(startingDelay).Append(
            DOTween.To(
                () => rect.sizeDelta.x,
                x => rect.sizeDelta = new Vector2(x, rect.sizeDelta.y),
                targetWidth,
                duration
            )
        );
    }

}


