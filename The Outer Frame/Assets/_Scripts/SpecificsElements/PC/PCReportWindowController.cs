using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabBtneport;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject panelReporte;
    [SerializeField] ReportController ReportToFill;
    bool isDeleted;
    WordData word;

    //OnSearchWord
    public void GetWord(Component sender, object _word)
    {
        if (_word == null) return;
        word = (WordData)_word;
    }

   //OnReportWindow
    public void InstanciateBtnReport(Component sender, object _word)
    {

        if (!word) return;
        foreach (Transform child in Grid.GetComponentsInChildren<Transform>())
        {
            if (child != Grid.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (isDeleted) return;
        //recorrer todos los reportes de la palabra, chequear que su state esté en el historial de visto, y pasar el reporte a 

        List<StateEnum> stateHistory = WordsManager.WM.GetHistorySeen(word);


        foreach (var state in stateHistory)
        {
            List<ReportType> reports = word.GetListOfReportFromState(state);
            foreach(ReportType R in reports)
            {
                if (!R.GetwasRegisteredInDB()) continue;
                GameObject btn = Instantiate(PrefabBtneport, Grid.transform, false);
                btn.GetComponent<PCReportController>().Inicialization(word, R);
            }
            
        }
    }

    //OnPressReportBTN
    public void SetPanelText(Component sender, object obj)
    {
        panelReporte.SetActive(true);

        ReportType report = (ReportType) obj;
        ReportToFill._Reset();
        ReportToFill.initReport(word, report, false, false, false, null, report.GetTimeWhenWasDone());
    }

    public void QuitPanelReport()
    {
        panelReporte.SetActive(false);
    }

    public void DeleteAllReports(Component sender, object obj)
    {
        isDeleted = true;
    }
}
