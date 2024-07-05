using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WordsManager : MonoBehaviour
{
    //Sistema encargado de solicitar requests a Words específicas para que devuelvan un input
    [SerializeField] List<WordData> wordsDic = new List<WordData>();
    [SerializeField] bool SaveProgress;
    public static WordsManager WM { get; private set; }
    public event Action<WordData> OnChangeStateOfWord;
    public event Action<WordData> OnChangeStateSeenOfWord;


    private void Awake()
    {
        if (WM != null && WM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            WM = this;
        }

        if (SaveProgress) return;
        foreach(WordData word in wordsDic)
        {
            word.CleanHistory();
        }
    }

    public ReportType RequestLastReport(WordData _word)
    {
        return FindWordInList(_word).GetLastReport();
    }

    public ReportType RequestReport(WordData _word, StateEnum state)
    {
        return FindWordInList(_word).GetReport(state);
    }

    public TVNewType RequestNew(WordData _word, StateEnum state)
    {
        return FindWordInList(_word).GetTVnew(state);
    }

    public DataBaseType RequestBDWikiData(WordData _word)
    {
        return FindWordInList(_word).GetDB();
    }

    public void RequestChangeState(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).ChangeState(WordState);
        OnChangeStateOfWord?.Invoke(_word);
    }

    public void RequestChangeStateSeen(WordData _word, StateEnum WordState)
    {
        FindWordInList(_word).CheckStateSeen(WordState);
        OnChangeStateSeenOfWord?.Invoke(_word);
    }

    public bool CheckIfStateWasDone(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).CheckIfStateWasDone(WordState);
    }

    public TimeData RequestTimeDataOfState(WordData _word, StateEnum WordState)
    {
        return FindWordInList(_word).GetTimeOfState(WordState);
    }
    public List<StateEnum> GetHistory(WordData _word)
    {
        return FindWordInList(_word).GetHistory();
    }

    WordData FindWordInList(WordData _word)
    {
        foreach(WordData w in wordsDic)
        {
            if(w==_word)
            {
                return w;
            }
        }

        return wordsDic[0];
    }

    public bool GetInactiveState(WordData _word)
    {
        return FindWordInList(_word).GetInactiveState();
    }

    public List<FindableWordData> SearchForFindableWord(TMP_Text textField)
    {
        List<FindableWordData> aux = new List<FindableWordData>();

        List<string> FindableWords = ExtractFindableWords(textField.text);

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
                        aux.Add(new FindableWordData(WordName, wordLocation, widthInfo,heigthInfo,e));
                    }
                    e++;
                }
            }
        }

        return aux;
    }

    List<string> ExtractFindableWords(string text)
    {
        List<string> aux = new List<string>();

        string pattern = @"<link>(.*?)<\/link>";

        MatchCollection coincidencias = Regex.Matches(text, pattern);

        foreach (Match coincidencia in coincidencias)
        {
            aux.Add(coincidencia.Groups[1].Value);
        }

        return aux;
    }

    public WordData FindWordDataWithString(string WordToCompare)
    {
        WordData WD = null;
        foreach(WordData w in wordsDic)
        {
            if(w.GetName().Contains(WordToCompare))
            {
                WD = w;
            }
        }
        return WD;
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
    public string GetName() { return name.GetName();     }
    public Vector3 GetPosition() { return position; }

    public float GetWidth() { return width; }
    public float GetHeigth() { return heigth; }
    public int GetWordIndex() { return wordIndex; }
}
