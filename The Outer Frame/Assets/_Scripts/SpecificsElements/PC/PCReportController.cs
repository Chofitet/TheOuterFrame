using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCReportController : MonoBehaviour
{
    [SerializeField] TMP_Text txt;

    string _input;
    GameObject _panel;

    public void Inicialization(Word.WordState state, TimeData time, string _word , GameObject Panel)
    {
        txt.text = state.ToString() + " " + time.ToString();
        _input = WordsManager.WM.RequestInputAccordingState(state,_word);
        _panel = Panel;
    }

    public void ShowPanelWithInput()
    {
        _panel.SetActive(true);
        _panel.GetComponentInChildren<TMP_Text>().text = _input;
    }
}
