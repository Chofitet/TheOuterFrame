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


    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Initialization(WordData Word, float Width, float Heigth, TMP_Text TextField)
    {
        rectTransform.sizeDelta = new Vector2(Width, Heigth);
        textField = TextField;
        word = Word;
        OriginalText = textField.text;
        ApplyEffectOnHover("#00F3FF");
    }

    public void PressButton()
    {
        OnPressHyperLink?.Invoke(this, word);
        Destroy(gameObject);
    }

    public void ApplyHover()
    {
        ApplyEffectOnHover("#1F9168");
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

            if (NormalizeWord(CleanUnnecessaryCharacter(combinedWord)).ToLower() == NormalizeWord(word.FindFindableName(CleanUnnecessaryCharacter(NormalizeWord(combinedWord)))).ToLower())
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

    string RemoveMaterialTags(string word)
    {
        return Regex.Replace(word, @"<\/?color.*?>", "");
    }

    string GetExtraCharacters(string word)
    {
        int endIndex = word.IndexOf("</u>", StringComparison.OrdinalIgnoreCase);
        if (endIndex != -1)
        {
            endIndex += "</u>".Length;
            if (endIndex < word.Length)
            {
                return word.Substring(endIndex);
            }
        }
        return "";
    }

    string NormalizeWord(string word)
    {
        return Regex.Replace(word, @"<\/?u>|[\?\.,\n\r]", "");
    }

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
        ApplyEffectOnHover("#00F3FF");
    }
}
