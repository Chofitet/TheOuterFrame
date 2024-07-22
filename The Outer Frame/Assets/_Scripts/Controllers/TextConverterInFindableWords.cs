using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextConverterInFindableWords : MonoBehaviour
{

    TMP_Text textfield;

    void Start()
    {
        textfield = GetComponent<TMP_Text>();

        FindableWordsManager.FWM.InstanciateFindableWord(textfield);
    }



}
