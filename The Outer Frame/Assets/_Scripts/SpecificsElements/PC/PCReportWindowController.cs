using System.Collections;
using System.Collections.Generic;
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

        List<StateEnum> stateHistory = WordsManager.WM.GetHistory(word);

        foreach (var state in stateHistory)
        {
            GameObject btn = Instantiate(PrefabBtneport, Grid.transform, false);
            btn.GetComponent<PCReportController>().Inicialization(state, word, panelReporte);
        }
    }

    public void QuitPanelReport()
    {
        panelReporte.SetActive(false);
    }

}
