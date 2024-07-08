using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindableWordBTNController : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] GameEvent OnFindableWordButtonPress;
    [SerializeField] GameEvent OnFindableWordButtonHover;
    [SerializeField] GameEvent OnFindableWordButtonUnHover;

    TMP_Text textField;
    WordData word;
    string OriginalText;
    int wordIndex;


    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialization(WordData Word, float Width, float Heigth, TMP_Text TextField, int WordIndex)
    {
        rectTransform.sizeDelta = new Vector2(Width, Heigth);
        textField = TextField;
        word = Word;
        OriginalText = textField.text;
        wordIndex = WordIndex;
    }

    public void ChangeToColorToHighligth()
    {
        OnFindableWordButtonHover?.Invoke(this, 4);
        BoldWord();
        //ChangeToColor(Color.red);
    }

    public void ChangeToColorToNormal()
    {
        OnFindableWordButtonUnHover?.Invoke(this, 3);
        UnBoldWord();
        //ChangeToColor(NormalColor);
    }

    void BoldWord()
    {
        string[] words = textField.text.Split(' ');
        string auxText = "";
        int i = 0;

        foreach (string w in words)
        {
            int iaux = i;
            string combinedWord = words[i];

            if (auxText.Contains(combinedWord))
            {
                i++;
                continue;
            }

            while (combinedWord.Contains("<link>") && !combinedWord.Contains("</link>") && iaux < words.Length - 1)
            {
                iaux++;
                combinedWord += " " + words[iaux];
            }

            if (combinedWord.Contains(word.GetName()))
            {
                StringBuilder strBuilder = new StringBuilder(combinedWord);
                strBuilder = strBuilder.Replace(combinedWord, "<material=\"LiberationSans Findable Word Effect\">" + combinedWord + "</material>");
                auxText += strBuilder + " ";
            }
            else
            {
                auxText += w + " ";
            }

            i++;

        }
        textField.text = auxText;
    }

    void UnBoldWord()
    {
        textField.text = OriginalText;
    }

    public void RegisterWord()
    {
        OnFindableWordButtonPress?.Invoke(this, word);
        UnBoldWord();
        Destroy(gameObject);
    }

    void ChangeToColor(Color color)
    {
        TMP_WordInfo info = textField.textInfo.wordInfo[wordIndex];
        for (int i = 0; i < info.characterCount; ++i)
        {
            int charIndex = info.firstCharacterIndex + i;
            int meshIndex = textField.textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = textField.textInfo.characterInfo[charIndex].vertexIndex;

            Color32[] vertexColors = textField.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;
        }

        textField.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}
