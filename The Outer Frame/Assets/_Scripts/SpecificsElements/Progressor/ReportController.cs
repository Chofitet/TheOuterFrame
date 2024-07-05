using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text reporttxt;
    [SerializeField] GameEventListener LeaveReportListener;
    [SerializeField] GameObject ButtonFindableWordPrefab;


    public void initReport(WordData word)
    {
        reporttxt.text = WordsManager.WM.RequestLastReport(word).GetText();
        
        List<FindableWordData> PositionsWord = WordsManager.WM.SearchForFindableWord(reporttxt);
        
        foreach (FindableWordData w in PositionsWord)
        {
           GameObject auxObj = Instantiate(ButtonFindableWordPrefab, w.GetPosition(), reporttxt.transform.rotation, reporttxt.transform);
           auxObj.GetComponent<FindableWordBTNController>().Initialization(w.GetWordData(),w.GetWidth(), w.GetHeigth(),reporttxt,w.GetWordIndex());
        }

    }

}
