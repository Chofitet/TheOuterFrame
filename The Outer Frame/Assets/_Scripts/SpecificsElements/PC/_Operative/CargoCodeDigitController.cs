using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoCodeDigitController : MonoBehaviour
{
    [SerializeField] List<Symbols> symbols = new List<Symbols>();
    [SerializeField] Image SymbolImage;
    int symbolIndex = 0;
    bool isInCorrectSymbol;

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

}

[Serializable]
public class Symbols
{
    public Sprite Symbol;
    public bool isCorrect;
}
