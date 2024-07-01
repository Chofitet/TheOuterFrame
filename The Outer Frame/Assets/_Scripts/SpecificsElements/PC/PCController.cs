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
    WordData word;

    public void CompleteSeachBar()
    {
        word = WordSelectedInNotebook.Notebook.GetSelectedWord();
        SearchBar.text = word.GetName();
    }

    public void SearchWordInWiki()
    {
        ShowWikiWindow();

        wikiData.text = WordsManager.WM.RequestBDWikiData(word).GetText();
        InstanciateBtnReport();
    }

    public void ShowWikiWindow()
    {
        WikiWindow.SetActive(true);
        ReportsWindow.SetActive(false);
        CallsWindow.SetActive(false);
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

        List<StateEnum> stateHistory = WordsManager.WM.GetHistory(word);

        foreach (var state in stateHistory)
        {
            GameObject btn = Instantiate(PrefabBtneport, Grid.transform,false);
            btn.GetComponent<PCReportController>().Inicialization(state, word, panelReporte);
        }
    }



}
