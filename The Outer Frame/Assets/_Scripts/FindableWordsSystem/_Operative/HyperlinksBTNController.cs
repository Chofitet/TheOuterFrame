using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class HyperlinksBTNController : MonoBehaviour
{
    RectTransform rectTransform;
    TMP_Text textField;
    WordData word;
    string OriginalText;
    [SerializeField] GameEvent OnPressHyperLink;
    bool _isRepitedButton;
    int wordIndex;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Initialization(WordData Word, float Width, float Heigth, TMP_Text TextField, bool isRepitedButton, int _wordIndex)
    {
        rectTransform.sizeDelta = new Vector2(Width, Heigth);
        textField = TextField;
        word = Word;
        OriginalText = textField.text;
        _isRepitedButton = isRepitedButton;
        Invoke("ApplyUnderline", 0.01f);
       
        wordIndex = _wordIndex;
    }

    public void PressButton()
    {
        OnPressHyperLink?.Invoke(this, word);
        Destroy(gameObject);
    }

    public void ApplyHover()
    {
        ApplyEffectOnHover("#00F3FF");
    }
    public void ApplyUnderline()
    {
        ApplyEffectOnHover("#00C3CC");
    }

    public void ApplyEffectOnHover(string color)
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

            while (combinedWord.Contains("<u>") && !combinedWord.Contains("</u>") && iaux < words.Length - 1)
            {
                iaux++;
                combinedWord += " " + words[iaux];
                extraIndex++;
            }
            string combinedWordClean = NormalizeWord(CleanUnnecessaryCharacter(RemoveMaterialTags(combinedWord))).ToLower();
            string FoundAs = NormalizeWord(word.FindFindableName(NormalizeWord(CleanUnnecessaryCharacter(RemoveMaterialTags(combinedWord))))).ToLower();

            combinedWordClean = Regex.Replace(combinedWordClean.Trim(), @"[^\w]", "");

            bool areClose = false;

            if ((combinedWordClean == FoundAs) && combinedWord.Contains("<u>"))
            {
                // probar si esta cerca del boton
                int currentIndex = 0;
                while (currentIndex < textField.textInfo.wordCount)
                {

                    int actualWordIndex = -1;
                    TMP_WordInfo currentWord = textField.textInfo.wordInfo[currentIndex];
                    string firstword = (NormalizeWord(CleanUnnecessaryCharacter(RemoveMaterialTags(currentWord.GetWord())))).ToLower();
                    string secondword = NormalizeWord(CleanUnnecessaryCharacter(RemoveMaterialTags(combinedWord))).ToLower();

                    if (secondword.Contains(firstword))
                    {
                        Vector3 wordLocation = textField.transform.TransformPoint(textField.textInfo.characterInfo[textField.textInfo.wordInfo[currentIndex].firstCharacterIndex].topLeft);
                        Vector3 btnLocation = transform.position;
                        float distanceBtnWord = Vector3.Distance(wordLocation, btnLocation);
                        float threshold = 0.00001f;

                        if (distanceBtnWord < threshold)
                        {
                            areClose = true;
                        }
                    }
                    currentIndex++;
                }
            }

            if (areClose) 
            {
                if (combinedWord.StartsWith("<color")) combinedWord = RemoveMaterialTags(combinedWord);

                string extraCharacters = GetExtraCharacters(combinedWord);
                StringBuilder strBuilder = new StringBuilder(combinedWord);
                if (combinedWord == "") return;
                strBuilder = strBuilder.Replace(combinedWord, "<color=" + color + ">" + CleanUnnecessaryCharacter(combinedWord) + "</color>");
                auxText += strBuilder + extraCharacters + " ";
            }
            else
            {
                auxText += combinedWord + " ";
            }

            i++;

        }
        textField.text = auxText;
        textField.ForceMeshUpdate();
    }

    string RemoveMaterialTags(string word) { return Regex.Replace(word, @"<\/?color.*?>", ""); }
    string GetExtraCharacters(string word) { int endIndex = word.IndexOf("</u>", StringComparison.OrdinalIgnoreCase); if (endIndex != -1) { endIndex += "</u>".Length; if (endIndex < word.Length) { return word.Substring(endIndex); } } return ""; }
    string NormalizeWord(string word) { return Regex.Replace(word, @"<\/?u>|[\?\.,\n\r]", ""); }
    

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

    

    public void UnapplyEffect()
    {
        ApplyEffectOnHover("#00C3CC");
    }

    public WordData Getword()
    {
        return word;
    }
}
