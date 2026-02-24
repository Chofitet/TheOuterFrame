using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public static class ProccessFindableWord 
{
    public static List<FindableWordData> SearchForFindableWord(TMP_Text textField, WordData irrelevant = null)
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

            /*if (registeredWords.Contains(word))
            {
                continue;
            }

            registeredWords.Add(word);*/

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
                if (i + 1 < wordCount && textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i].firstCharacterIndex].lineNumber != textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].lineNumber && i + 1 != wordCount)
                {

                    combinedWordLength = combinedWordLength + spaceToAdd;
                    heightInfo += heightInfo / 4;
                    checkSlicebtn = true;
                    findableWords.Add(new FindableWordData(word, wordLocation, combinedWordLength, heightInfo, checkSlicebtn, startIndex, irrelevant));
                    wordLocation = textField.transform.TransformPoint(
                    textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].topLeft);
                    combinedWordLength = 0;
                    spaceToAdd = 0;
                    heightInfo = 0;
                }

            }

            combinedWordLength = combinedWordLength + spaceToAdd;
            heightInfo += heightInfo / 4;

            findableWords.Add(new FindableWordData(word, wordLocation, combinedWordLength, heightInfo, checkSlicebtn, startIndex, irrelevant));
        }

        return findableWords;
    }

    static string AddCustomTagsToLinks(string originalText)
    {
        string noSpecialChars = RemoveSpecialCharacters(originalText);
        string normalizedSpaces = NormalizeSpaces(noSpecialChars);
        string replacedLinks = ReplaceLinkTags(normalizedSpaces);

        return replacedLinks;
    }

    // 1) Remover “ ” ' "
    static readonly char[] specialChars = { '“', '”', '"', '\'' };

    static string RemoveSpecialCharacters(string input)
    {
        StringBuilder sb = new StringBuilder(input.Length);

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            bool remove = false;

            for (int j = 0; j < specialChars.Length; j++)
            {
                if (c == specialChars[j])
                {
                    remove = true;
                    break;
                }
            }

            if (!remove)
                sb.Append(c);
        }

        return sb.ToString();
    }

    // 2) Normalizar espacios (equivalente a Regex "\s+")
    static string NormalizeSpaces(string input)
    {
        StringBuilder sb = new StringBuilder(input.Length);
        bool previousWasSpace = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsWhiteSpace(c))
            {
                if (!previousWasSpace)
                    sb.Append(' ');

                previousWasSpace = true;
            }
            else
            {
                previousWasSpace = false;
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    // 3) Reemplazar <link>...</link> por ii...ij
    static string ReplaceLinkTags(string text)
    {
        StringBuilder sb = new StringBuilder(text.Length);
        int i = 0;

        while (i < text.Length)
        {
            // Detectar <link>
            if (i + 6 <= text.Length && text.Substring(i, 6) == "<link>")
            {
                i += 6;           // saltar "<link>"
                sb.Append("ii");  // abrir tag personalizado

                // Copiar contenido interno hasta </link>
                while (i + 7 <= text.Length && text.Substring(i, 7) != "</link>")
                {
                    sb.Append(text[i]);
                    i++;
                }

                sb.Append("ij");  // cerrar tag personalizado

                i += 7;           // saltar "</link>"
            }
            else
            {
                sb.Append(text[i]);
                i++;
            }
        }

        return sb.ToString();


    }
}

[Serializable]
public class FindableWordData
{
    string Name;
    WordData wordIgnore;
    Vector3 position;
    float width;
    float heigth;
    bool isRepitedButton;
    int wordIndex;
    public FindableWordData(string _name, Vector3 _position, float _with, float _heigth, bool _isRepitedButton, int _wordIndex, WordData _WordIgnore = null)
    {
        Name = _name;
        position = _position;
        width = _with;
        heigth = _heigth;
        isRepitedButton = _isRepitedButton;
        wordIndex = _wordIndex;
    }
    public WordData GetWordData() { return WordsManager.WM.FindWordDataWithString(Name, wordIgnore); }
    public string GetName()
    {
        return Name;
    }
    public Vector3 GetPosition() { return position; }

    public float GetWidth() { return width; }
    public float GetHeigth() { return heigth; }

    public bool GeisRepitedButton() { return isRepitedButton; }
    public int GetWordIndex() { return wordIndex; }

}
