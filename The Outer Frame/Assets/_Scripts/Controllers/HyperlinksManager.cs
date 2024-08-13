using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HyperlinksManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonHyperLinkPrefab;
    public static HyperlinksManager HLM { get; private set; }
    List<GameObject> HyperLinkBTNs = new List<GameObject>();

    private void Awake()
    {
        if (HLM != null && HLM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            HLM = this;
        }
    }
    public void InstanciateHyperLink(TMP_Text textField)
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
                if (child.GetComponent<HyperlinksManager>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            List<FindableWordData> PositionsWord = SearchForHyperLinkWord(textField);

            foreach (FindableWordData w in PositionsWord)
            {
                Debug.Log("HyperLink found: " + w.GetWordData().GetName());
                GameObject auxObj = Instantiate(ButtonHyperLinkPrefab, w.GetPosition(), textField.transform.rotation, textField.transform);
                auxObj.GetComponent<HyperlinksBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField);
                HyperLinkBTNs.Add(auxObj);
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


    List<FindableWordData> SearchForHyperLinkWord(TMP_Text textField)
    {
        List<FindableWordData> aux = new List<FindableWordData>();

        if (textField.IsActive()) textField.ForceMeshUpdate();

        string[] words = textField.text.Split(' ');
        List<int> processedIndices = new List<int>();

        for (int i = 0; i < words.Length; i++)
        {
            int amoutOfWordindex = 0;
            if (processedIndices.Contains(i)) continue;

            string combinedWord = words[i];
            int startIndex = i;

            while (combinedWord.Contains("<u>") && !combinedWord.Contains("</u>") && i < words.Length - 1)
            {
                i++;
                combinedWord += " " + words[i];
                amoutOfWordindex++;
            }

            if (combinedWord.Contains("</u>"))
            {
                combinedWord = CleanUnnecessaryCharacter(combinedWord);
                combinedWord = Regex.Replace(combinedWord, @"<\/?u>", "");
                processedIndices.AddRange(Enumerable.Range(startIndex, i - startIndex + 1));

                float CombinedWordLength = 0;
                float heigthInfo = 0;
                var wordLocation = Vector3.zero;
                int e = 0;
                int o = 0;
                bool IsInWord = false;
                foreach (TMP_WordInfo wordInfo in textField.textInfo.wordInfo)
                {
                    if (wordInfo.characterCount == 0 || string.IsNullOrEmpty(wordInfo.GetWord()))
                        continue;

                    string NormalizedCombinedWord = NormalizeWord(combinedWord);
                    string NormalizedWordInfo = NormalizeWord(wordInfo.GetWord());

                    if (NormalizedCombinedWord.StartsWith(NormalizedWordInfo) || IsInWord)
                    {
                        IsInWord = true;
                        var firstCharInfo = textField.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                        var lastCharInfo = textField.textInfo.characterInfo[wordInfo.lastCharacterIndex];
                        if (o == 0) wordLocation = textField.transform.TransformPoint((firstCharInfo.topLeft));
                        CombinedWordLength += Math.Abs((float)(firstCharInfo.topLeft.x - lastCharInfo.topRight.x));
                        heigthInfo = Math.Abs(firstCharInfo.topLeft.y - firstCharInfo.bottomLeft.y);
                        int numOfWord = wordInfo.characterCount;
                        o++;
                    }
                    e++;

                    if (NormalizedCombinedWord.EndsWith(NormalizedWordInfo)) IsInWord = false;
                }
                heigthInfo = heigthInfo + heigthInfo / 4;
                aux.Add(new FindableWordData(WordWithoutPointLineBreak(combinedWord), wordLocation, CombinedWordLength, heigthInfo, e));
            }
        }

        return aux;
    }

    string CleanUnnecessaryCharacter(string word)
    {
        int endIndex = word.IndexOf("</u>", StringComparison.OrdinalIgnoreCase);
        if (endIndex != -1)
        {
            endIndex += "</u>".Length;
            word = word.Substring(0, endIndex);
        }

        return word;
    }

    string NormalizeWord(string word)
    {
        return Regex.Replace(word.ToLower(), @"<\/?u>|[\?\.,\n\r\(\)\s]", "");
    }

    string WordWithoutPointLineBreak(string word)
    {
        return Regex.Replace(word, @"[.,\n\r]", "");
    }
}
