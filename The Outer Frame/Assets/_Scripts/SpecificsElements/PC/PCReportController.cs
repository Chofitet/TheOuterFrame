using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportController : MonoBehaviour
{
    [SerializeField] TMP_Text txt;
    [SerializeField] GameEvent OnPressPCReportBTN;

    string _input;

    public void Inicialization(StateEnum state, WordData _word)
    {
        ReportType input = WordsManager.WM.RequestReport(_word, state);
        txt.text = state.name + input.GetTimeWhenWasDone().ToString();
        _input = input.GetText();
    }

    public void ShowPanelWithInput()
    {
        OnPressPCReportBTN?.Invoke(this, _input);
    }

}
