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

    [Header("Remove from Irrelevant")]
    [SerializeField] WordData Irrelevant;
    [SerializeField] List<DataRemoveIrrelevant> DataToUpdateIrrelevantDB = new List<DataRemoveIrrelevant>();

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += RemoveFindableAsToIrrelevant;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= RemoveFindableAsToIrrelevant;
    }

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
    public void InstanciateHyperLink(TMP_Text textField, FindableBtnType btnType)
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

        RemoveFindableAsToIrrelevant();

        try
        {
            foreach (Transform child in textField.transform)
            {
                if (child.GetComponent<HyperlinksBTNController>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            List<FindableWordData> PositionsWord = SearchForHyperLinkWord(textField);

            foreach (FindableWordData w in PositionsWord)
            {
                Debug.Log("HyperLink found: " + w.GetWordData().GetName());
                GameObject auxObj = Instantiate(ButtonHyperLinkPrefab, w.GetPosition(), textField.transform.rotation, textField.transform);
                auxObj.GetComponent<HyperlinksBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GeisRepitedButton());
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
        List<FindableWordData> findableWords = new List<FindableWordData>();

        string originalText = textField.text;
        string auxiliaryText = AddCustomTagsToLinks(textField.text); //Replace <link> and </link> to "ii" and "ij"
        textField.text = auxiliaryText;
        HashSet<string> registeredWords = new HashSet<string>();
        List<string> CleanWords = new List<string>();

        if (textField.IsActive()) textField.ForceMeshUpdate();

        Dictionary<int, int> wordRanges = new Dictionary<int, int>();

        int currentIndex = 0;
        while (currentIndex < textField.textInfo.wordCount)
        {
            TMP_WordInfo currentWord = textField.textInfo.wordInfo[currentIndex];

            if (currentWord.GetWord().StartsWith("ii"))
            {
                //Find word start with <link>
                int startIndex = currentIndex;
                int wordCount = 0;


                while (currentIndex < textField.textInfo.wordCount)
                {
                    TMP_WordInfo wordInRange = textField.textInfo.wordInfo[currentIndex];
                    wordCount++;
                    //count length of combined word

                    if (wordInRange.GetWord() == "ii") wordCount--;

                    if (wordInRange.GetWord().Contains("ij"))
                    {
                        //find word finish with </link>
                        if (wordInRange.GetWord() == "ij") wordCount--;
                        CleanWords.Add(CleanUpAfterTag(wordInRange.GetWord()));
                        break;
                    }
                    currentIndex++;
                }

                // Add to dictionary first word's index and the count of words that have the combined word
                wordRanges.Add(startIndex, wordCount);
            }
            currentIndex++;

        }

        // Back to original text
        textField.text = originalText;
        if (textField.IsActive()) textField.ForceMeshUpdate();

        // Recorrer el diccionario y calcular la información de FindableWordData
        foreach (var entry in wordRanges)
        {
            int startIndex = entry.Key;
            int wordCount = entry.Value;

            int spaceToAdd = 0;
            bool checkSlicebtn = false;

            // Position of btn
            Vector3 wordLocation = textField.transform.TransformPoint(
            textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex].firstCharacterIndex].topLeft);
            float combinedWordLength = 0;
            float heightInfo = 0;
            string word = "";

            for (int i = 0; i < wordCount; i++)
            {
                TMP_WordInfo wordInfo = textField.textInfo.wordInfo[startIndex + i];
                word += wordInfo.GetWord() + " ";
            }

            word = word.TrimEnd();

            /* (registeredWords.Contains(word))
            {
                continue;
            }

            registeredWords.Add(word);*/

            for (int i = 0; i < wordCount; i++)
            {
                TMP_WordInfo wordInfo = textField.textInfo.wordInfo[startIndex + i];
                var firstCharInfo = textField.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                var lastCharInfo = textField.textInfo.characterInfo[wordInfo.lastCharacterIndex];
                spaceToAdd++;
                // Length of btn
                combinedWordLength += Math.Abs(firstCharInfo.topLeft.x - lastCharInfo.topRight.x);
                // heightInfo of btn
                heightInfo = Math.Max(heightInfo, Math.Abs(firstCharInfo.topLeft.y - firstCharInfo.bottomLeft.y));

                //  check to slice btn in differents text lines
                if (textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i].firstCharacterIndex].lineNumber != textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].lineNumber && i + 1 != wordCount)
                {

                    combinedWordLength = combinedWordLength + spaceToAdd;
                    heightInfo += heightInfo / 4;
                    checkSlicebtn = true;
                    findableWords.Add(new FindableWordData(SearchCleanedWord(CleanWords, word), wordLocation, combinedWordLength, heightInfo, checkSlicebtn));
                    wordLocation = textField.transform.TransformPoint(
                    textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].topLeft);
                    combinedWordLength = 0;
                    spaceToAdd = 0;
                    heightInfo = 0;
                }

            }

            combinedWordLength = combinedWordLength + spaceToAdd;
            heightInfo += heightInfo / 4;

            findableWords.Add(new FindableWordData(SearchCleanedWord(CleanWords,word), wordLocation, combinedWordLength, heightInfo, checkSlicebtn));
        }

        return findableWords;
    }

    string AddCustomTagsToLinks(string originalText)
    {
        string normalizedText = Regex.Replace(originalText, @"[“”\""\']", string.Empty);
        normalizedText = Regex.Replace(normalizedText, @"\s+", " ");

        return Regex.Replace(normalizedText, @"<u>(.*?)<\/u>", "ii$1ij");
    }

    string CleanUpAfterTag(string word)
    {
        if (word.StartsWith("ii"))
        {
            word = word.Substring(2); // Elimina los primeros 2 caracteres "ii"
        }

        int index = word.IndexOf("ij");
        if (index != -1)
        {
            return word.Substring(0, index); // +2 para incluir "ij"
        }
        return word.Trim(); 
    }

    string SearchCleanedWord(List<string> list, string word)
    {
        if (word == "VTTD") return "VTTD";
        // Dividimos la palabra en un array por los espacios
        string[] words = word.Split(' ');

        // Buscamos en la lista si alguna palabra coincide con la última palabra de 'words'
        for (int i = 0; i < list.Count; i++)
        {
            if (words[words.Length - 1].Contains(list[i]))
            {
                // Reemplazar la última palabra por la coincidencia encontrada
                words[words.Length - 1] = list[i];
                break;
            }
        }

        // Volvemos a unir las palabras en una cadena
        return string.Join(" ", words);
    }

    void RemoveFindableAsToIrrelevant()
    {
        foreach(DataRemoveIrrelevant data in DataToUpdateIrrelevantDB)
        {
            if(CheckConditionals(data.Conditions))
            {
                Irrelevant.DeleteFoundAsWord(data.FindableAsToRemove);
            }
        }
    }

    public bool CheckConditionals(List<ConditionalClass> list)
    {
        //es el estado default
        if (list == null) return true;

        foreach (ConditionalClass conditional in list)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition(3);

            //Debug.Log("last compete conditional of " + auxInterface + " is " + auxInterface.GetLastCompletedConditional());

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

            return true;
    }
}
[Serializable]
public class DataRemoveIrrelevant
{
    public string FindableAsToRemove;
    public List<ConditionalClass> Conditions;
}
