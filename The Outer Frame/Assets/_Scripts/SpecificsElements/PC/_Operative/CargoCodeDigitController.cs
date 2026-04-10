using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoCodeDigitController : MonoBehaviour
{
    [SerializeField] List<Symbols> symbols = new List<Symbols>();
    [SerializeField] Image SymbolImage;
    [SerializeField] Image BackImage;
    [SerializeField] Button UpBTN;
    [SerializeField] Button DownBTN;
    int symbolIndex = 0;
    bool isInCorrectSymbol;
    private void Start()
    {
        SymbolImage.sprite = symbols[0].Symbol;
    }

    public void PressUpBTN()
    {
        symbolIndex -= 1;

        if(symbolIndex < 0)
        {
            symbolIndex = symbols.Count - 1;
        }
        SymbolImage.sprite = symbols[symbolIndex].Symbol;
        isInCorrectSymbol = symbols[symbolIndex].isCorrect;
    }

    public void PressDownBTN()
    {
        symbolIndex += 1;

        if (symbolIndex > symbols.Count - 1)
        {
            symbolIndex = 0;
        }
        SymbolImage.sprite = symbols[symbolIndex].Symbol;
        isInCorrectSymbol = symbols[symbolIndex].isCorrect;
    }

    public bool GetIsInCorrectSymbol() { return isInCorrectSymbol; }

    public void InactiveBackPanel()
    {
        BackImage.color = new Color(BackImage.color.r, BackImage.color.g, BackImage.color.b, 0.5f);
        UpBTN.interactable = false;
        DownBTN.interactable = false;
    }
    public void ActiveBackPanel()
    {
        BackImage.color = new Color(BackImage.color.r, BackImage.color.g, BackImage.color.b, 1);
        UpBTN.interactable = true;
        DownBTN.interactable = true;
    }



}

[Serializable]
public class Symbols
{
    public Sprite Symbol;
    public bool isCorrect;
}
