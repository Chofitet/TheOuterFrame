using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindableWordsManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonFindableWordPrefab;
    [SerializeField] GameObject ButtonHyperLink;
    List<GameObject> FindableWordsBTNs = new List<GameObject>();
    [SerializeField] GameEvent OnFindableWordInstance;
    [SerializeField] WordData Irrelevant;
   
    
    public static FindableWordsManager FWM { get; private set; }

    private void Awake()
    {
        if (FWM != null && FWM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            FWM = this;
        }
    }

    public void InstanciateFindableWord(TMP_Text textField, FindableBtnType btnType, bool _comesFromDBTitle = false)
    {
        // btnType para cuando quiera refactorizar este script para que funcione con links tambien

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
                if (child.GetComponent<FindableWordBTNController>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            List<FindableWordData> PositionsWord = SearchForFindableWord(textField);

            foreach (FindableWordData w in PositionsWord)
            {
                if (w.GetWordData().GetIsFound()) continue;
                GameObject auxObj = Instantiate(ButtonFindableWordPrefab, w.GetPosition(), textField.transform.rotation, textField.transform);
                auxObj.name = "FindableBTN_" + w.GetWordData().GetName();
                auxObj.GetComponent<FindableWordBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GeisRepitedButton(), _comesFromDBTitle);
                FindableWordsBTNs.Add(auxObj);
                OnFindableWordInstance?.Invoke(this, auxObj);
                auxObj.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(auxObj));
            }
        } catch (Exception ex)
        {
            Debug.LogError("Error instantiating findable words: " + ex.Message);
        }

        foreach (var obj in deactivatedParents)
        {
            obj.SetActive(false);
        }

    }
    List<FindableWordData> SearchForFindableWord(TMP_Text textField)
    {
        List<FindableWordData> findableWords = new List<FindableWordData>();

        string originalText = textField.text;
        string auxiliaryText = AddCustomTagsToLinks(textField.text); //Replace <link> and </link> to "ii" and "ij"
        textField.text = auxiliaryText;
        HashSet<string> registeredWords = new HashSet<string>();

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

                    if (wordInRange.GetWord().EndsWith("ij"))
                    {
                        //find word finish with </link>
                        if (wordInRange.GetWord() == "ij") wordCount--;
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

                if (registeredWords.Contains(word))
                {
                    continue;
                }

                registeredWords.Add(word);

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
                    if (textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i].firstCharacterIndex].lineNumber != textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i+1].firstCharacterIndex].lineNumber && i+1 != wordCount)
                    {
                    
                        combinedWordLength = combinedWordLength + spaceToAdd;
                        heightInfo += heightInfo / 4;
                        checkSlicebtn = true;
                        findableWords.Add(new FindableWordData( word,wordLocation, combinedWordLength,heightInfo, checkSlicebtn, Irrelevant));
                        wordLocation = textField.transform.TransformPoint(
                        textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].topLeft);
                        combinedWordLength = 0;
                        spaceToAdd = 0;
                        heightInfo = 0;
                    }
                
                }

                combinedWordLength = combinedWordLength + spaceToAdd;
                heightInfo += heightInfo / 4;

                findableWords.Add(new FindableWordData(word,wordLocation,combinedWordLength,heightInfo,checkSlicebtn, Irrelevant));
            }

        return findableWords;
    }

    string AddCustomTagsToLinks(string originalText)
    {
        string normalizedText = Regex.Replace(originalText, @"[“”\""\']", string.Empty);
        normalizedText = Regex.Replace(normalizedText, @"\s+", " ");

        return Regex.Replace(normalizedText, @"<link>(.*?)<\/link>", "ii$1ij");
    }

    void OnButtonClick(GameObject obj)
    {
        DeleteBtnAlreadyFound(obj.GetComponent<FindableWordBTNController>().Getword());
    }

    void DeleteBtnAlreadyFound(WordData newWord)
    {
        foreach(GameObject btn in FindableWordsBTNs)
        {
            if (btn.GetComponent<FindableWordBTNController>().Getword() == newWord)
            {
                Destroy(btn);
            }
        }
    }


    int index;
    private void Update()
    {
        FindableWordsBTNs.RemoveAll(s => s == null);

        foreach (GameObject fw in FindableWordsBTNs)
        {
            WordData word = fw.GetComponent<FindableWordBTNController>().Getword();
            if (word.GetIsFound())
            {
                Destroy(fw);
            }
            else if(word.GetIsPhoneNumberFound() && word.GetIsAPhoneNumber())
            {
                Destroy(fw);
            }
        }
   }
   

}
public enum FindableBtnType
{
    FindableBTN,
    HyperLink
}
public struct FindableWordData
{
    WordData name;
    Vector3 position;
    float width;
    float heigth;
    bool isRepitedButton;

    public FindableWordData(string _name, Vector3 _position, float _with, float _heigth, bool _isRepitedButton, WordData _WordIgnore = null)
    {
        name = WordsManager.WM.FindWordDataWithString(_name, _WordIgnore);
        position = _position;
        width = _with;
        heigth = _heigth;
        isRepitedButton = _isRepitedButton;
    }
    public WordData GetWordData() { return name; }
    public string GetName() {
        if (!name) return "";
        return name.GetName(); 
    }
    public Vector3 GetPosition() { return position; }

    public float GetWidth() { return width; }
    public float GetHeigth() { return heigth; }

    public bool GeisRepitedButton() { return isRepitedButton; }

}
