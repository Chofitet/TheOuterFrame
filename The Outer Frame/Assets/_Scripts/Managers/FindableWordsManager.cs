using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class FindableWordsManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonFindableWordPrefab;
    ViewStates ActualViewState;
    List<GameObject> FindableWordsBTNs = new List<GameObject>();
    bool isHover;
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
        List<FindableWordData> PositionsWord = SearchForFindableWord(textField);

        foreach (FindableWordData w in PositionsWord)
        {
            if (w.GetWordData().GetIsFound()) return;
            GameObject auxObj = Instantiate(ButtonFindableWordPrefab, w.GetPosition(), textField.transform.rotation, textField.transform);
            auxObj.GetComponent<FindableWordBTNController>().Initialization(w.GetWordData(), w.GetWidth(), w.GetHeigth(), textField, w.GetWordIndex());
            FindableWordsBTNs.Add(auxObj);
        }
    }

    public List<FindableWordData> SearchForFindableWord(TMP_Text textField)
    {
        List<FindableWordData> aux = new List<FindableWordData>();

        textField.ForceMeshUpdate();

        string[] words = textField.text.Split(' ');
        List<int> processedIndices = new List<int>();

        for (int i = 0; i < words.Length; i++)
        {
            if (processedIndices.Contains(i)) continue;

            string combinedWord = words[i];
            int startIndex = i;

            while (combinedWord.Contains("<link>") && !combinedWord.Contains("</link>") && i < words.Length - 1)
            {
                i++;
                combinedWord += " " + words[i];
            }

            if (combinedWord.Contains("</link>"))
            {
                combinedWord = Regex.Replace(combinedWord, @"<\/?link>", "");
                processedIndices.AddRange(Enumerable.Range(startIndex, i - startIndex + 1));

                int e = 0;
                foreach (TMP_WordInfo wordInfo in textField.textInfo.wordInfo)
                {
                    if (wordInfo.characterCount == 0 || string.IsNullOrEmpty(wordInfo.GetWord()))
                        continue;
                    if (combinedWord.Contains(wordInfo.GetWord()))
                    {
                        var firstCharInfo = textField.textInfo.characterInfo[wordInfo.firstCharacterIndex];
                        var lastCharInfo = textField.textInfo.characterInfo[wordInfo.lastCharacterIndex];
                        var wordLocation = textField.transform.TransformPoint((firstCharInfo.topLeft + lastCharInfo.bottomRight) / 2f);
                        float widthInfo = Math.Abs((float)(firstCharInfo.topLeft.x - lastCharInfo.topLeft.x));
                        widthInfo = widthInfo + widthInfo / 4;
                        float heigthInfo = Math.Abs(firstCharInfo.topLeft.y - firstCharInfo.bottomLeft.y);
                        heigthInfo = heigthInfo + heigthInfo / 4;
                        int numOfWord = wordInfo.characterCount;
                        string WordName = wordInfo.GetWord();
                        aux.Add(new FindableWordData(WordName, wordLocation, widthInfo, heigthInfo, e));
                    }
                    e++;
                }
            }
        }

        return aux;
    }

    float BTNdistance = 1000;
    private void Update()
    {
        if (isHover) return;
        if(ActualViewState == ViewStates.GeneralView)
        {
            ChangeCusorIcon(null, 0);
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
        if (index is WordData) index = 0;
        int i = (int)index;

        Cursor.SetCursor(CursorTextures[i], new Vector2(32, 32), CursorMode.ForceSoftware);

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
    public string GetName() { return name.GetName(); }
    public Vector3 GetPosition() { return position; }

    public float GetWidth() { return width; }
    public float GetHeigth() { return heigth; }
    public int GetWordIndex() { return wordIndex; }
}