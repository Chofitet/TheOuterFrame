using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionRowController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtext;
    [SerializeField] GameObject strikethrough;
    [SerializeField] TMP_Text ActionText;
    [SerializeField] Toggle toggle;
    [SerializeField] Button btn;
    StateEnum state;

    public void Initialization(StateEnum _state)
    {
        state = _state;
        ActionText.text = _state.GetActionVerb();
        btn.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (!WordSelectedInNotebook.Notebook.GetSelectedWord()) return;
        Wordtext.text = WordSelectedInNotebook.Notebook.GetSelectedWord().GetName();
        toggle.isOn = true;
    }

    public Button GetButton() { return btn; }

    public StateEnum GetState() { return state; }

    public void ResetRow()
    {
        toggle.isOn = false;
        Wordtext.text = "";
    }

    public void DesactiveRow()
    {
        btn.enabled = false;
        strikethrough.SetActive(true);

    }

    
}
