using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabBtneport;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject panelReporte;

    public void InstanciateBtnReport(Component sender, object _word)
    {
        WordData word = (WordData)_word;

        foreach (Transform child in Grid.GetComponentsInChildren<Transform>())
        {
            if (child != Grid.transform)
            {
                Destroy(child.gameObject);
            }
        }

        List<StateEnum> stateHistory = WordsManager.WM.GetHistorySeen(word);

        foreach (var state in stateHistory)
        {
            GameObject btn = Instantiate(PrefabBtneport, Grid.transform, false);
            btn.GetComponent<PCReportController>().Inicialization(state, word);
        }
    }

    public void SetPanelText(Component sender, object obj)
    {
        panelReporte.SetActive(true);

        string report = (string) obj;

        TMP_Text panelText = panelReporte.transform.GetChild(0).GetComponent<TMP_Text>();
        panelText.text = report;
    }

    public void QuitPanelReport()
    {
        panelReporte.SetActive(false);
    }

}
