using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabBtneport;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject panelReporte;

    WordData word;

    //OnSearchWord
    public void GetWord(Component sender, object _word)
    {
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

        //recorrer todos los reportes de la palabra, chequear que su state esté en el historial de visto, y pasar el reporte a 

        List<StateEnum> stateHistory = WordsManager.WM.GetHistorySeen(word);


        foreach (var state in stateHistory)
        {
            GameObject btn = Instantiate(PrefabBtneport, Grid.transform, false);
            btn.GetComponent<PCReportController>().Inicialization(word, WordsManager.WM.RequestSpecificReport(word,state));
        }
    }

    public void SetPanelText(Component sender, object obj)
    {
        panelReporte.SetActive(true);

        string report = (string) obj;

        TMP_Text panelText = panelReporte.transform.GetChild(0).GetComponent<TMP_Text>();
        panelText.text = report;
        FindableWordsManager.FWM.InstanciateFindableWord(panelText);
    }

    public void QuitPanelReport()
    {
        panelReporte.SetActive(false);
    }

}
