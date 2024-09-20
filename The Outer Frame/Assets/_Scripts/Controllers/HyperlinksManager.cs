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
                if (child.GetComponent<HyperlinksBTNController>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            List<FindableWordData> PositionsWord = SearchForHyperLinkWord(textField,false);

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


    List<FindableWordData> SearchForHyperLinkWord(TMP_Text textField, bool applyXCorrection)
    {
        List<FindableWordData> aux = new List<FindableWordData>();

        string OriginalText = textField.text;
        string auxiliaryText = AddCustomTagsToLinks(textField.text);
        textField.text = auxiliaryText;


        if (textField.IsActive()) textField.ForceMeshUpdate();

        string[] words = auxiliaryText.Split(' ');
        List<int> processedIndices = new List<int>();
        List<Vector3> SavedPositions = new List<Vector3>();
        List<TMP_WordInfo> listOfWords = new List<TMP_WordInfo>();

        for (int i = 0; i < words.Length; i++)
        {
            int amoutOfWordindex = 0;
            if (processedIndices.Contains(i)) continue;

            string combinedWord = words[i];
            int startIndex = i;


            while (combinedWord.Contains("ii") && !combinedWord.Contains("ij") && i < words.Length - 1)
            {
                i++;
                combinedWord += " " + words[i];
                amoutOfWordindex++;
            }

            if (combinedWord.Contains("ii"))
            {
                combinedWord = CleanUnnecessaryCharacter(combinedWord);
                combinedWord = Regex.Replace(combinedWord, @"\/?ij", "");
                processedIndices.AddRange(Enumerable.Range(startIndex, i - startIndex + 1));

                float CombinedWordLength = 0;
                float heigthInfo = 0;
                var wordLocation = Vector3.zero;
                int e = 0;
                int WordInfoCount = 0;
                int o = 0;
                int v = 0;
                bool IsInWord = false;
                string auxWordToCompare = "";
                bool BreakLineInstanciate = false;

                foreach (TMP_WordInfo wordInfo in textField.textInfo.wordInfo)
                {
                    WordInfoCount++;
                    if (wordInfo.characterCount == 0 || string.IsNullOrEmpty(wordInfo.GetWord()))
                        continue;
                    string NormalizedWordInfo = wordInfo.GetWord();
                    if (NormalizedWordInfo.StartsWith("ii") || IsInWord || BreakLineInstanciate)
                    {
                        IsInWord = true;
                        NormalizedWordInfo = NormalizeWord(NormalizedWordInfo);
                        var firstCharInfo = textField.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                        var lastCharInfo = textField.textInfo.characterInfo[wordInfo.lastCharacterIndex];

                        if (o == 0)
                        {
                            wordLocation = textField.transform.TransformPoint(firstCharInfo.topLeft);
                            if (SavedPositions.Any(p => Vector3.Distance(p, wordLocation) < 0.001f))
                            {
                                // Debug.Log("Skipping duplicate word at position: " + wordLocation);
                                IsInWord = false;
                                continue;
                            }
                            SavedPositions.Add(wordLocation);
                        }
                        CombinedWordLength += Math.Abs(firstCharInfo.topLeft.x - lastCharInfo.topRight.x);
                        heigthInfo = Math.Abs(firstCharInfo.topLeft.y - firstCharInfo.bottomLeft.y);
                        auxWordToCompare += wordInfo.GetWord();
                        o++;
                        v++;
                        textField.text = auxiliaryText;

                        if (NormalizedWordInfo.EndsWith("ij"))
                        {
                            IsInWord = false;
                            BreakLineInstanciate = false;
                            listOfWords.Add(wordInfo);
                            break;
                        }
                        else if (firstCharInfo.lineNumber != textField.textInfo.characterInfo[textField.textInfo.wordInfo[WordInfoCount].firstCharacterIndex].lineNumber)
                        {
                            if (NormalizedWordInfo.EndsWith("ij"))
                            {
                                IsInWord = false;
                                BreakLineInstanciate = false;
                                break;
                            }
                            IsInWord = false;
                            heigthInfo = heigthInfo + heigthInfo / 4;
                            CombinedWordLength = CombinedWordLength - 3 + v;
                            aux.Add(new FindableWordData(WordWithoutPointLineBreak(combinedWord), wordLocation, CombinedWordLength, heigthInfo, e));
                            o = 0;
                            CombinedWordLength = 0;
                            BreakLineInstanciate = true;
                            continue;
                        }
                    }
                    e++;
                }
                heigthInfo = heigthInfo + heigthInfo / 4;
                CombinedWordLength = CombinedWordLength - 5 + v;
                aux.Add(new FindableWordData(WordWithoutPointLineBreak(combinedWord), wordLocation, CombinedWordLength, heigthInfo, e));
            }
        }
        textField.text = OriginalText;
        return aux;
    }

    string AddCustomTagsToLinks(string originalText)
    {
        return Regex.Replace(originalText, @"<u>(.*?)<\/u>", "ii$1ij");
    }

    string CleanUnnecessaryCharacter(string word)
    {
        int startIndex = word.IndexOf("ii", StringComparison.OrdinalIgnoreCase);
        if (startIndex != -1)
        {
            word = word.Substring(startIndex + "ii".Length);
        }

        int endIndex = word.IndexOf("ij", StringComparison.OrdinalIgnoreCase);
        if (endIndex != -1)
        {
            word = word.Substring(0, endIndex).Trim();
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
