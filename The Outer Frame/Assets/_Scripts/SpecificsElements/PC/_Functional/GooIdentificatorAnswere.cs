using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GooIdentificatorAnswere : MonoBehaviour
{
    [SerializeField] TMP_Text optionLetter;

    public void init(int letterIndex)
    {

        optionLetter.text = chooseLetterByIndex(letterIndex);
    }

    string chooseLetterByIndex(int index)
    {
        switch (index)
        {
            case 0: return "A";
            case 1: return "B";
            case 2: return "C";
            case 3: return "D";
            case 4: return "E";
            case 5: return "F";
            case 6: return "G";
            case 7: return "H";
            case 8: return "I";
            case 9: return "J";
            default: return "A";    
        }
    }
}
