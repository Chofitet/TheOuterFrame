using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PCController : MonoBehaviour
{
    [SerializeField] TMP_Text SearchBar;
    [SerializeField] GameObject WikiWindow;
    [SerializeField] GameObject ReportsWindow;
    [SerializeField] GameObject CallsWindow;
    [SerializeField] TMP_Text wikiData;

    [SerializeField] GameObject PrefabBtneport;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject panelReporte;
    string word;

    public void CompleteSeachBar()
    {
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();
        SearchBar.text = word;
    }

    public void SearchWordInWiki()
    {
        WikiWindow.SetActive(true);
        ReportsWindow.SetActive(false);
        CallsWindow.SetActive(false);

        wikiData.text = WordsManager.WM.RequestBDWikiData(word);
        InstanciateBtnReport();
    }

    public void ShowReportsWindow()
    {
        WikiWindow.SetActive(false);
        ReportsWindow.SetActive(true);
        CallsWindow.SetActive(false);
    }

    public void QuitPanelReport()
    {
        panelReporte.SetActive(false);
    }


    void InstanciateBtnReport()
    {
        foreach (Transform child in Grid.GetComponentsInChildren<Transform>())
        {
            if (child != Grid.transform) 
            {
                Destroy(child.gameObject);
            }
        }

        Dictionary<Word.WordState, TimeData> stateTimeHistory = WordsManager.WM.RequestStateTimeHistory(word);

        foreach (var key in stateTimeHistory.Keys)
        {
            GameObject btn = Instantiate(PrefabBtneport, Grid.transform,false);
            btn.GetComponent<PCReportController>().Inicialization(key, stateTimeHistory[key], word, panelReporte);
        }
    }



}
