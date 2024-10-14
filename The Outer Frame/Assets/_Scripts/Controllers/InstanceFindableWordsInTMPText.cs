using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class InstanceFindableWordsInTMPText : MonoBehaviour
{
    TMP_Text textField;
    private void Awake()
    {
        textField = GetComponent<TMP_Text>();
        FindableWordsManager.FWM.InstanciateFindableWord(textField, FindableBtnType.FindableBTN);
    }
}
