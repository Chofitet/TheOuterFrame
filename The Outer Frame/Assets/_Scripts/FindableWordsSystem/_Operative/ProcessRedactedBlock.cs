using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class ProcessRedactedBlock
{
    public static List<RedactedBlockData> SearchForRedactedBlocks(TMP_Text textField, bool applyXCorrection)
    {
        List<RedactedBlockData> aux = new List<RedactedBlockData>();

        textField.textInfo.Clear();

        string[] words = textField.text.Split(' ');
        int WordsCount = words.Length;


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
                wordLocation = firstCharInfo.topLeft;
                aux.Add(new RedactedBlockData(wordLocation, wordInfo.GetWord()));

                e++;
            }
            i++;
        }
        return aux;
    }
}

[Serializable]
public class RedactedBlockData
{
    public Vector3 position;
    public string redactedText;

    public RedactedBlockData(Vector3 _position, string _redactedText)
    {
        position = _position;
        redactedText = _redactedText;
    }
}

