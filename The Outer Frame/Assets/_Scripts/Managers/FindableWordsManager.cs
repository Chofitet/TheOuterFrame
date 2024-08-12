using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindableWordsManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonFindableWordPrefab;
    ViewStates ActualViewState;
    List<GameObject> FindableWordsBTNs = new List<GameObject>();
    bool isHover;
    bool isOnFindableMode;
    [SerializeField] Texture2D[] CursorTextures;
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

    public void SetActualViewState(Component sender, object viewState)
    {
        ActualViewState = (ViewStates)viewState;
    }

    public void InstanciateFindableWord(TMP_Text textField)
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

            List<FindableWordData> PositionsWord = SearchForFindableWord(textField);

            foreach (FindableWordData w in PositionsWord)
            {
                if (w.GetWordData().GetIsFound()) continue;
                GameObject auxObj = Instantiate(ButtonFindableWordPrefab, w.GetPosition(), textField.transform.rotation, textField.transform);
                auxObj.GetComponent<FindableWordBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GetWordIndex());
                FindableWordsBTNs.Add(auxObj);

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
        List<FindableWordData> aux = new List<FindableWordData>();

        if(textField.IsActive()) textField.ForceMeshUpdate();

        string[] words = textField.text.Split(' ');
        List<int> processedIndices = new List<int>();

        for (int i = 0; i < words.Length; i++)
        {
            int amoutOfWordindex = 0;
            if (processedIndices.Contains(i)) continue;

            string combinedWord = words[i];
            int startIndex = i;

            while (combinedWord.Contains("<link>") && !combinedWord.Contains("</link>") && i < words.Length - 1)
            {
                i++;
                combinedWord += " " + words[i];
                amoutOfWordindex++;
            }

            if (combinedWord.Contains("</link>"))
            {
                combinedWord = CleanUnnecessaryCharacter(combinedWord);
                combinedWord = Regex.Replace(combinedWord, @"<\/?link>", "");
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

                    string NormalizedCombinedWord =  NormalizeWord(combinedWord);
                    string NormalizedWordInfo = NormalizeWord(wordInfo.GetWord());

                    if (NormalizedCombinedWord.StartsWith(NormalizedWordInfo) || IsInWord)
                    {
                        IsInWord = true;
                        var firstCharInfo = textField.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                        var lastCharInfo = textField.textInfo.characterInfo[wordInfo.lastCharacterIndex];
                        if(o == 0) wordLocation = textField.transform.TransformPoint((firstCharInfo.topLeft));
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



        return CleanListOfRepeatedWords(aux);
    }

    string CleanUnnecessaryCharacter(string word)
    {
        int endIndex = word.IndexOf("</link>", StringComparison.OrdinalIgnoreCase);
        if (endIndex != -1)
        {
            endIndex += "</link>".Length;
            word = word.Substring(0, endIndex);
        }

        return word;
    }

    string NormalizeWord(string word)
    {
        return Regex.Replace(word.ToLower(), @"<\/?link>|[\?\.,\n\r\(\)\s]", "");
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

    float BTNdistance = 1000;

    int index;
    private void Update()
    {
        FindableWordsBTNs.RemoveAll(s => s == null);

        if (index != FindableWordsBTNs.Count) isHover = false;
        index = FindableWordsBTNs.Count;

       if (isHover || !isOnFindableMode)
        {
            return;
        }
        
        GetClosestButton();

        if (BTNdistance < 70)
        {
            ChangeCusorIcon(null, 3);
        }
        else if (BTNdistance < 120)
        {
            ChangeCusorIcon(null, 2);
        }
        else if (BTNdistance < 200)
        {
            ChangeCusorIcon(null, 1);
        }
        else ChangeCusorIcon(null, 0);

    }

    public void ChangeCusorIcon(Component sender, object index)
    {
        if (!isOnFindableMode) return;
        if (index is WordData) index = 0;
        int i = (int)index;

        Cursor.SetCursor(CursorTextures[i], new Vector2(16, 16), CursorMode.ForceSoftware);

        if (i == 3) isHover = false;
        else if (i == 4) isHover = true;

    }
    void GetClosestButton()
    {
        Vector3 mousePosition = Input.mousePosition;
        GameObject closestButton = null;

        float minDistance = float.MaxValue;
        FindableWordsBTNs.RemoveAll(s => s == null);

        foreach (GameObject btn in FindableWordsBTNs)
        {
            if (!btn.GetComponent<FindableWordBTNController>().GetIsVisible()) continue;
            Vector3 btnScreenPosition = Camera.main.WorldToScreenPoint(btn.transform.position);
            float distance = Vector3.Distance(mousePosition, btnScreenPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestButton = btn;
            }
        }

        if (closestButton != null)
        {
            Vector3 btnScreenPosition = Camera.main.WorldToScreenPoint(closestButton.transform.position);
            BTNdistance = Vector3.Distance(mousePosition, btnScreenPosition);
        }
        else BTNdistance = 1000;

    }

    public void SetFindableMode(Component sender, object obj)
    {
        if (isOnFindableMode)
        {
            StartCoroutine(delayFindableMode(0.1f, false));
            
        }
        else StartCoroutine(delayFindableMode(0.3f, true));
    }

    IEnumerator delayFindableMode(float time, bool x)
    {
        yield return new WaitForSeconds(time);
        isOnFindableMode = x;
        yield return new WaitForSeconds(0.1f);
        if (!isOnFindableMode) ChangeCusorIcon(null, 0); 
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
