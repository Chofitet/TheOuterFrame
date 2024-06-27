using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text reporttxt;


    public void initReport(string word)
    {
        reporttxt.text = WordsManager.WM.RequestLastReport(word).GetText();

    }

}
