using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public static class ProccessHyperLinks
{
    public static List<FindableWordData> SearchForHyperLinkWord(TMP_Text textField, WordData irrelevant = null)
    {
        List<FindableWordData> findableWords = new List<FindableWordData>();

        string originalText = textField.text;
        string auxiliaryText = AddCustomTagsToLinks(textField.text); //Replace <link> and </link> to "ii" and "ij"
        textField.text = auxiliaryText;
        HashSet<string> registeredWords = new HashSet<string>();
        List<string> CleanWords = new List<string>();

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

                if (currentWord.GetWord().Contains("Km"))
                {
                    Debug.Log("a");
                }

                while (currentIndex < textField.textInfo.wordCount)
                {
                    TMP_WordInfo wordInRange = textField.textInfo.wordInfo[currentIndex];
                    wordCount++;
                    //count length of combined word

                    if (wordInRange.GetWord() == "ii") wordCount--;

                    if (wordInRange.GetWord().Contains("ij"))
                    {
                        //find word finish with </link>
                        if (wordInRange.GetWord() == "ij") wordCount--;
                        CleanWords.Add(CleanUpAfterTag(wordInRange.GetWord()));
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
            Vector3 wordLocation = textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex].firstCharacterIndex].topLeft;
            float combinedWordLength = 0;
            float heightInfo = 0;
            string word = "";



            for (int i = 0; i < wordCount; i++)
            {
                TMP_WordInfo wordInfo = textField.textInfo.wordInfo[startIndex + i];
                word += wordInfo.GetWord() + " ";
            }

            word = word.TrimEnd();


            /* (registeredWords.Contains(word))
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

                if (i + 1 != wordCount) //no estoy en la última palabra
                {
                    int actualWordLineNumber = textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i].firstCharacterIndex].lineNumber;
                    int nextWordLineNumber = textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].lineNumber;
                    //  check to slice btn in differents text lines
                    if (actualWordLineNumber != nextWordLineNumber)
                    {

                        combinedWordLength = combinedWordLength + spaceToAdd;
                        heightInfo += heightInfo / 4;
                        checkSlicebtn = true;
                        findableWords.Add(new FindableWordData(SearchCleanedWord(CleanWords, word), wordLocation, combinedWordLength, heightInfo, checkSlicebtn, startIndex));
                        wordLocation = textField.textInfo.characterInfo[textField.textInfo.wordInfo[startIndex + i + 1].firstCharacterIndex].topLeft;
                        combinedWordLength = 0;
                        spaceToAdd = 0;
                        heightInfo = 0;
                    }
                }

            }

            combinedWordLength = combinedWordLength + spaceToAdd;
            heightInfo += heightInfo / 4;

            findableWords.Add(new FindableWordData(SearchCleanedWord(CleanWords, word), wordLocation, combinedWordLength, heightInfo, checkSlicebtn, startIndex));
        }

        return findableWords;
    }

    static string AddCustomTagsToLinks(string originalText)
    {
        if (string.IsNullOrEmpty(originalText))
            return originalText;

        // 1. Remover comillas
        string cleaned = RemoveQuotes(originalText);

        // 2. Normalizar espacios
        cleaned = NormalizeSpaces(cleaned);

        // 3. Reemplazar <u>...</u> → ii...ij
        cleaned = ReplaceUnderlineTags(cleaned);

        return cleaned;
    }

    static string RemoveQuotes(string input)
    {
        Span<char> buffer = stackalloc char[input.Length];
        int count = 0;

        foreach (char c in input)
        {
            if (c != '“' && c != '”' && c != '"' && c != '\'')
                buffer[count++] = c;
        }

        return new string(buffer[..count]);
    }

    static string NormalizeSpaces(string input)
    {
        StringBuilder sb = new StringBuilder(input.Length);
        bool lastWasSpace = false;

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!lastWasSpace)
                {
                    sb.Append(' ');
                    lastWasSpace = true;
                }
            }
            else
            {
                sb.Append(c);
                lastWasSpace = false;
            }
        }

        return sb.ToString();
    }

    static string ReplaceUnderlineTags(string input)
    {
        StringBuilder sb = new StringBuilder(input.Length);

        for (int i = 0; i < input.Length; i++)
        {
            if (i + 3 < input.Length && input[i] == '<' && input[i + 1] == 'u' && input[i + 2] == '>')
            {
                sb.Append("ii");
                i += 3;

                while (i + 4 < input.Length &&
                       !(input[i] == '<' && input[i + 1] == '/' && input[i + 2] == 'u' && input[i + 3] == '>'))
                {
                    sb.Append(input[i]);
                    i++;
                }

                sb.Append("ij");
                i += 3;
            }
            else
            {
                sb.Append(input[i]);
            }
        }

        return sb.ToString();
    }

    static string CleanUpAfterTag(string word)
    {
        if (word.StartsWith("ii"))
        {
            word = word.Substring(2); // Elimina los primeros 2 caracteres "ii"
        }

        int index = word.IndexOf("ij");
        if (index != -1)
        {
            return word.Substring(0, index); // +2 para incluir "ij"
        }
        return word.Trim();
    }

    static string SearchCleanedWord(List<string> list, string word)
    {
        if (word == "VTTD") return "VTTD";
        // Dividimos la palabra en un array por los espacios
        string[] words = word.Split(' ');

        // Buscamos en la lista si alguna palabra coincide con la última palabra de 'words'
        for (int i = 0; i < list.Count; i++)
        {
            if (words[words.Length - 1].Contains(list[i]))
            {
                // Reemplazar la última palabra por la coincidencia encontrada
                words[words.Length - 1] = list[i];
                break;
            }
        }

        // Volvemos a unir las palabras en una cadena
        return string.Join(" ", words);
    }

   
}
