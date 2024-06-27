using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportController : MonoBehaviour
{
    [SerializeField] TMP_Text txt;

    string _input;
    GameObject _panel;

    public void Inicialization(StateEnum state, string _word , GameObject Panel)
    {
        ReportType input = WordsManager.WM.RequestReport(_word, state);
        txt.text = state.name + input.GetTimeWhenWasDone().ToString();
        _input = input.GetText();
        _panel = Panel;
    }

    public void ShowPanelWithInput()
    {
        _panel.SetActive(true);
        _panel.GetComponentInChildren<TMP_Text>().text = _input;
    }
}
