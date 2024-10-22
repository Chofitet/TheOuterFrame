using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InstanciateRedactedBlock : MonoBehaviour
{
    [SerializeField] GameObject redactedBlockPrefab;
    List<GameObject> RedactedBlockList = new List<GameObject>();

    public static InstanciateRedactedBlock IRM { get; private set; }

    private void Awake()
    {
        if (IRM != null && IRM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            IRM = this;
        }
    }

    public void InstanciateRedactedBlocks(TMP_Text textField)
    {
        if (textField == null)
        {
            Debug.LogError("TextField is null");
            return;
        }

        List<GameObject> deactivatedParents = new List<GameObject>();
        Transform currentTransform = textField.transform;
        while (currentTransform != null)
        {
            if (!currentTransform.gameObject.activeSelf)
            {
                currentTransform.gameObject.SetActive(true);
                deactivatedParents.Add(currentTransform.gameObject);
            }
            currentTransform = currentTransform.parent;
        }

        try
        {
            foreach (Transform child in textField.transform)
            {
                if (child.GetComponent<RedactedBlock>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
            RedactedBlockList.Clear();

            List<RedactedBlockData> PositionsWord = SearchForRedactedBlocks(textField, false);

            foreach (RedactedBlockData w in PositionsWord)
            {
                GameObject auxObj = Instantiate(redactedBlockPrefab, w.position, textField.transform.rotation, textField.transform);
                auxObj.GetComponent<RedactedBlock>().Initialization(w.redactedText);
                RedactedBlockList.Add(auxObj);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error instantiating findable words: " + ex.Message);
        }

        foreach (var obj in deactivatedParents)
        {
            obj.SetActive(false);
        }

    }


    List<RedactedBlockData> SearchForRedactedBlocks(TMP_Text textField, bool applyXCorrection)
    {
        List<RedactedBlockData> aux = new List<RedactedBlockData>();

        textField.textInfo.Clear();

        string[] words = textField.text.Split(' ');
        int WordsCount = words.Length;

        Debug.Log(textField.text);

        if (textField.IsActive())
        {
            textField.ForceMeshUpdate();
        }
        var wordLocation = Vector3.zero;
            int e = 0;
            int i = 0;

       foreach (TMP_WordInfo wordInfo in textField.textInfo.wordInfo)
       {
            int wordDiference = textField.textInfo.wordCount;
            if (i >= WordsCount - (WordsCount - wordDiference)) break;
            if (wordInfo.characterCount == 0 || string.IsNullOrEmpty(wordInfo.GetWord())) continue;
            string actualWord = wordInfo.GetWord();
            if (wordInfo.GetWord() == "REDACTED" || wordInfo.GetWord() == "RE" || wordInfo.GetWord() == "REDACTEDTO" || wordInfo.GetWord() == "REDA")
            {
                var firstCharInfo = textField.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                var lastCharInfo = textField.textInfo.characterInfo[wordInfo.lastCharacterIndex];
                wordLocation = textField.transform.TransformPoint(firstCharInfo.topLeft);
                aux.Add(new RedactedBlockData(wordLocation, wordInfo.GetWord()));
                    
                e++;
            }
            i++;
        }
        return aux;
    }
}

class RedactedBlockData
{
    public Vector3 position;
    public string redactedText;

    public RedactedBlockData(Vector3 _position, string _redactedText)
    {
        position = _position;
        redactedText = _redactedText;
    }
}