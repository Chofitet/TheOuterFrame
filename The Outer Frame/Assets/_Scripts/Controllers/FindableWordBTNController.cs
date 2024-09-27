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
        wordIndex = WordIndex;
        
        transform.localPosition = transform.localPosition - new Vector3(IndexFilter(), 0, 0);
        ApplyShader("Bold");
    }

    float IndexFilter()
    {
        switch(wordIndex)
        {
            case 1: return 0; break;
            case 2: return 4; break;
            case 3: return 9; break;
            case 4: return 18; break;
        }

        return 0;
    }

    public void ChangeToColorToHighligth()
    {
        OnFindableWordButtonHover?.Invoke(this, 4);
        Debug.Log("hover");
        ApplyShader("Red");
    }

    public void ChangeToColorToNormal()
    {
        OnFindableWordButtonUnHover?.Invoke(this, 3);
        UnBoldWord();
    }

    void ApplyShader(string MaterialName)
    {
        string[] words = textField.text.Split(' ');
        string auxText = "";
        int i = 0;
        int extraIndex = 0;

        foreach (string w in words)
        {
            if (w == "")
            {
                i++;
                continue;
            }
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

            string combinedWordClean = NormalizeWord(CleanUnnecessaryCharacter(combinedWord)).ToLower();
            string FoundAs = NormalizeWord(word.FindFindableName(CleanUnnecessaryCharacter(NormalizeWord(combinedWord)))).ToLower();

            if ((combinedWordClean == FoundAs) && combinedWord.Contains("link"))
            {
                if (combinedWord.StartsWith("<material")) combinedWord = RemoveMaterialTags(combinedWord);

                string extraCharacters = GetExtraCharacters(combinedWord);
                StringBuilder strBuilder = new StringBuilder(combinedWord);

                string materialName = string.Empty;
                try
                {
                    materialName = "\"" + textField.font.name + "" + MaterialName;
                    strBuilder = strBuilder.Replace(combinedWord, "<material=" + materialName + ">" + CleanUnnecessaryCharacter(combinedWord) + "</material>");
                    Debug.Log(materialName);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Error al obtener el path del material: " + ex.Message);
                    strBuilder = strBuilder.Replace(combinedWord,  CleanUnnecessaryCharacter(combinedWord));
                }
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
    string RemoveMaterialTags(string word)
    {
        return Regex.Replace(word, @"<\/?material.*?>", "");
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
        string _word;
        _word = Regex.Replace(word, @"<\/?link>|[\?\.,\n\r]", "");
        return RemoveMaterialTags(_word);
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
        ApplyShader("Bold");
    }

    public void RegisterWord()
    {
        OnFindableWordButtonPress?.Invoke(this, word);
        ApplyShader("Grey");
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
    public WordData Getword() { return word; }
}
