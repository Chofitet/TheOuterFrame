using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Xml;

[RequireComponent(typeof(ShaderMaterialManager))]
public class FindableWordBTNController : MonoBehaviour, IFindableBTN
{
    RectTransform rectTransform;

    [SerializeField] GameEvent OnFindableWordButtonPress;
    [SerializeField] GameEvent OnFindableWordButtonHover;
    [SerializeField] GameEvent OnFindableWordButtonUnHover;
    [SerializeField] WordData TheCabin;
    bool isInactive;

    TMP_Text textField;
    WordData word;
    WordData wordToPass;
    int wordIndex;

    bool wasFinded;
    bool once;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        if (wasFinded || _isRepitedButton) return;
        ApplyShader("");
    }

    bool _isRepitedButton;
    bool _comesFromDBTitle;
    public void Initialization(WordData Word, float Width, float Heigth, TMP_Text TextField, bool isRepitedButton, bool comesFromDBTitle = false)
    {
        rectTransform.sizeDelta = new Vector2(Width, Heigth);
        textField = TextField;
        word = Word;
        wordToPass = word;
        _isRepitedButton = isRepitedButton;
        _comesFromDBTitle = comesFromDBTitle;
        ApplyShader("Bold");
    }

    public void ChangeToColorToHighligth()
    {
        if (isInactive) return;
        OnFindableWordButtonHover?.Invoke(this, 4);
        ApplyShader("Red");
        GlowOn();
    }

    public void ChangeToColorToNormal()
    {
        if (isInactive) return;
        OnFindableWordButtonUnHover?.Invoke(this, 3);
        GlowOff();
    }

    void ApplyShader(string MaterialName, bool eraceSpace = true)
    {
        if (isInactive) return;
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
            string FoundAs = NormalizeWord(word.FindFindableName(NormalizeWord(CleanUnnecessaryCharacter(combinedWord)), _comesFromDBTitle)).ToLower();

            combinedWordClean = Regex.Replace(combinedWordClean.Trim(), @"[^\w]", "");

            if ((combinedWordClean == FoundAs) && combinedWord.Contains("link"))
            {
                if (combinedWord.StartsWith("<material")) combinedWord = RemoveMaterialTags(combinedWord);

                string extraCharacters = GetExtraCharacters(combinedWord);
                //Debug.Log(FoundAs + extraCharacters);
                StringBuilder strBuilder = new StringBuilder(combinedWord);

                string materialName = string.Empty;

                if (MaterialName != "")
                {
                    try
                    {
                        materialName = "\"" + textField.font.name + "" + MaterialName;
                        strBuilder = strBuilder.Replace(combinedWord, "<material=" + materialName + ">" + CleanUnnecessaryCharacter(combinedWord) + "</material>");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Error al obtener el path del material: " + ex.Message);
                        strBuilder = strBuilder.Replace(combinedWord, CleanUnnecessaryCharacter(combinedWord));
                    }
                }

                string stringToAdd = strBuilder + extraCharacters + " ";
                stringToAdd = stringToAdd.Replace(",,", ",");
                stringToAdd = stringToAdd.Replace("..", ".");

                auxText += stringToAdd;
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
        _word = Regex.Replace(word, @"<\/?link>|[\?\.,\n\r\s-]", "");
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

    public void RegisterWord()
    {
        OnFindableWordButtonPress?.Invoke(this, wordToPass);
        ApplyShader("Grey");
        wasFinded = true;
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

    public void ReplaceWordToCabin(Component sender, object obj)
    {
        ApplyShader("");
        isInactive = true;
        wordToPass = TheCabin;
    }

    private Sequence glowSequence;

    public void GlowOn()
    {
        Material mat = GetMat();

        // Cancelamos cualquier animación previa
        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        glowSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_OutlineWidth),
                x => mat.SetFloat(ShaderUtilities.ID_OutlineWidth, x),
                0.25f,   
                0.15f    
            ).SetEase(Ease.InOutSine)
        );
    }

    public void GlowOff()
    {
        Material mat = GetMat();

        glowSequence?.Kill();

        glowSequence = DOTween.Sequence();

        float enValue = 0.2f; //

        glowSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_OutlineWidth),
                x => mat.SetFloat(ShaderUtilities.ID_OutlineWidth, x),
                enValue,      
                0.15f   
            ).SetEase(Ease.InOutSine)
        );

        glowSequence.OnComplete(() => ApplyShader("Bold"));
    }

    private Material GetMat()
    {
        return GetComponent<ShaderMaterialManager>().GetHighLigthMaterial(textField.font.name);
    }

}

