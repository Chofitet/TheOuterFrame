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
        textField.ForceMeshUpdate();

        string originalText = textField.text;
        StringBuilder result = new StringBuilder();

        int currentIndex = 0;

        while (currentIndex < originalText.Length)
        {
            int underlineStart = originalText.IndexOf(
                "<u>",
                currentIndex,
                StringComparison.OrdinalIgnoreCase
            );

            if (underlineStart == -1)
            {
                result.Append(originalText.Substring(currentIndex));
                break;
            }

            int contentStart = underlineStart + "<u>".Length;

            int underlineEnd = originalText.IndexOf(
                "</u>",
                contentStart,
                StringComparison.OrdinalIgnoreCase
            );

            if (underlineEnd == -1)
            {
                result.Append(originalText.Substring(currentIndex));
                break;
            }

            // Conservar exactamente lo que hay antes del hyperlink
            result.Append(originalText.Substring(
                currentIndex,
                underlineStart - currentIndex
            ));

            string content = originalText.Substring(
                contentStart,
                underlineEnd - contentStart
            );

            string fullUnderline = originalText.Substring(
                underlineStart,
                underlineEnd + "</u>".Length - underlineStart
            );

            string cleanContent = RemoveMaterialTags(
                CleanUnnecessaryCharacter(fullUnderline)
            );

            string combinedWordClean = NormalizeWord(cleanContent).ToLower();

            string foundAs = NormalizeWord(
                word.FindFindableName(
                    NormalizeWord(cleanContent)
                )
            ).ToLower();

            combinedWordClean = Regex.Replace(
                combinedWordClean.Trim(),
                @"[^\w]",
                ""
            );

            foundAs = Regex.Replace(
                foundAs.Trim(),
                @"[^\w]",
                ""
            );

            bool areClose = false;

            if (combinedWordClean == foundAs)
            {
                for (int i = 0; i < textField.textInfo.wordCount; i++)
                {
                    TMP_WordInfo currentWord =
                        textField.textInfo.wordInfo[i];

                    string firstWord = NormalizeWord(
                        CleanUnnecessaryCharacter(
                            RemoveMaterialTags(currentWord.GetWord())
                        )
                    ).ToLower();

                    string secondWord = NormalizeWord(
                        CleanUnnecessaryCharacter(
                            RemoveMaterialTags(fullUnderline)
                        )
                    ).ToLower();

                    if (secondWord.Contains(firstWord))
                    {
                        Vector3 wordLocation =
                            textField.transform.TransformPoint(
                                textField.textInfo.characterInfo[
                                    currentWord.firstCharacterIndex
                                ].topLeft
                            );

                        float distance = Vector3.Distance(
                            wordLocation,
                            transform.position
                        );

                        if (distance < 0.00001f)
                        {
                            areClose = true;
                            break;
                        }
                    }
                }
            }

            if (areClose)
            {
                result.Append(
                    "<color=" + color + ">" +
                    fullUnderline +
                    "</color>"
                );
            }
            else
            {
                result.Append(fullUnderline);
            }

            currentIndex = underlineEnd + "</u>".Length;
        }

        textField.text = result.ToString();
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
