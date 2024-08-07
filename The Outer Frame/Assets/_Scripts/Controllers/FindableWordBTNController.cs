using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FindableWordBTNController : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] GameEvent OnFindableWordButtonPress;
    [SerializeField] GameEvent OnFindableWordButtonHover;
    [SerializeField] GameEvent OnFindableWordButtonUnHover;
    [SerializeField] LayerMask visibleLayerMask;
    [SerializeField] LayerMask ignoreLayerMask;

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
    }

    public void ChangeToColorToNormal()
    {
        OnFindableWordButtonUnHover?.Invoke(this, 3);
        UnBoldWord();
    }

    void BoldWord()
    {
        string[] words = textField.text.Split(' ');
        string auxText = "";
        int i = 0;
        int extraIndex = 0;

        foreach (string w in words)
        {
            int iaux = i;
            string combinedWord = words[i];

            if (extraIndex > 0)
            {
                i++;
                extraIndex--;
                continue;
            }

            while (combinedWord.Contains("<link>") && !combinedWord.Contains("</link>") && iaux < words.Length - 1)
            {
                iaux++;
                combinedWord += " " + words[iaux];
                extraIndex++;
            }

            if (NormalizeWord(CleanUnnecessaryCharacter(combinedWord)) == NormalizeWord(word.GetFindableName()))
            {
                string extraCharacters = GetExtraCharacters(combinedWord);
                StringBuilder strBuilder = new StringBuilder(combinedWord);
                string materialName = "\"" + textField.font.name + " Effect\"";
                strBuilder = strBuilder.Replace(combinedWord, "<material=" +materialName+ ">" + CleanUnnecessaryCharacter(combinedWord) + "</material>");
                auxText += strBuilder + extraCharacters + " ";
            }
            else
            {
                auxText += combinedWord + " ";
            }

            i++;

        }
        textField.text = auxText;
    }

    string GetExtraCharacters(string word)
    {
        int endIndex = word.IndexOf("</link>", StringComparison.OrdinalIgnoreCase);
        if (endIndex != -1)
        {
            endIndex += "</link>".Length;
            if (endIndex < word.Length)
            {
                return word.Substring(endIndex);
            }
        }
        return "";
    }

    string NormalizeWord(string word)
    {
        return Regex.Replace(word.ToLower(), @"<\/?link>|[\?\.,\n\r]", "");
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


    private bool IsVisible()
    {
        Ray ray = new Ray(Camera.main.transform.position, (transform.position - Camera.main.transform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == gameObject)
            {
                
                return true;
            }
        }

        return false;
    }

    public bool GetIsVisible() { return IsVisible(); }
    public TMP_Text GetTextField() { return textField; }
}
