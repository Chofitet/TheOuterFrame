using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

interface IFindableBTN 
{
    void Initialization(WordData Word, float Width, float Heigth, TMP_Text TextField, bool isRepitedButton);

    WordData Getword();
}
