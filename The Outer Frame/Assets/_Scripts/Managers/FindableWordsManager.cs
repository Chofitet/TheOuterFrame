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
    List<GameObject> FindableWordsBTNs = new List<GameObject>();
    [SerializeField] GameEvent OnFindableWordInstance;
    
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

    public void InstanciateFindableWord(TMP_Text textField, bool applyXCorrection = false)
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
                if (child.GetComponent<FindableWordBTNController>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            List<FindableWordData> PositionsWord = SearchForFindableWord(textField, applyXCorrection);

            foreach (FindableWordData w in PositionsWord)
            {
                if (w.GetWordData().GetIsFound()) continue;
                GameObject auxObj = Instantiate(ButtonFindableWordPrefab, w.GetPosition(), textField.transform.rotation, textField.transform);
                auxObj.GetComponent<FindableWordBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GetWordIndex());
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

    List<FindableWordData> SearchForFindableWord(TMP_Text textField, bool applyXCorrection)
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
                aux.Add(new FindableWordData(WordWithoutPointLineBreak(combinedWord), wordLocation, CombinedWordLength, heigthInfo, XCorrectionPosition(textField, listOfWords, listOfWords.Last(), applyXCorrection)));
            }
        }
        textField.text = OriginalText;
        return aux;
    }

    string AddCustomTagsToLinks(string originalText)
    {
        return Regex.Replace(originalText, @"<link>(.*?)<\/link>", "ii$1ij");
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

    int XCorrectionPosition(TMP_Text textField, List<TMP_WordInfo> otherwords, TMP_WordInfo actualWord, bool applyXCorrection)
    {
        if (!applyXCorrection) return 1;
        int aux = 1;
        foreach(TMP_WordInfo otherW in otherwords)
        {
            string a = otherW.GetWord();
            string b = actualWord.GetWord();
            Debug.Log(otherW.GetWord());
            if (otherW.GetWord() == actualWord.GetWord()) return aux;
            if(AreInTheSameParagraph(textField, otherW,actualWord))
            {

                aux++;
            }
        }

        return aux;
    }

    bool AreInTheSameParagraph( TMP_Text textField, TMP_WordInfo firstCharInfo, TMP_WordInfo lastCharInfo)
    {
        int firstCharLine = textField.textInfo.characterInfo[firstCharInfo.firstCharacterIndex].lineNumber;
        int lastCharLine = textField.textInfo.characterInfo[lastCharInfo.firstCharacterIndex].lineNumber;

        if (firstCharLine != lastCharLine)
        {
            for (int i = firstCharInfo.lastCharacterIndex; i < lastCharInfo.firstCharacterIndex; i++)
            {
                char currentChar = textField.textInfo.characterInfo[i].character;
                if (currentChar == '\n')
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return true;
        }
    }

    string WordWithoutPointLineBreak(string word)
    {
        return Regex.Replace(word, @"[.,\n\r]", "");
    }

    public List<FindableWordData> CleanListOfRepeatedWords(List<FindableWordData> list)
    {
        Dictionary<string, FindableWordData> uniqueWords = new Dictionary<string, FindableWordData>();
        foreach (var item in list)
        {
            if (item.GetName() == "") continue;
            string name = item.GetName();
            if (!uniqueWords.ContainsKey(name))
            {
                uniqueWords[name] = item;
            }
        }
        return new List<FindableWordData>(uniqueWords.Values);
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
            if (fw.GetComponent<FindableWordBTNController>().Getword().GetIsFound())
            {
                Destroy(fw);
            }
        }
   }
   

}
public struct FindableWordData
{
    WordData name;
    Vector3 position;
    float width;
    float heigth;
    int wordIndex;

    public FindableWordData(string _name, Vector3 _position, float _with, float _heigth, int _wordIndex)
    {
        name = WordsManager.WM.FindWordDataWithString(_name);
        position = _position;
        width = _with;
        heigth = _heigth;
        wordIndex = _wordIndex;
    }
    public WordData GetWordData() { return name; }
    public string GetName() {
        if (!name) return "";
        return name.GetName(); 
    }
    public Vector3 GetPosition() { return position; }

    public float GetWidth() { return width; }
    public float GetHeigth() { return heigth; }
    public int GetWordIndex() { return wordIndex; }

}
