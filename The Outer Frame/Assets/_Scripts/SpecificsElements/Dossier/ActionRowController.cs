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
    [SerializeField] GameEvent OnShakeNotebook;
    StateEnum state;

    public void Initialization(StateEnum _state)
    {
        state = _state;
        ActionText.text = _state.GetActionVerb();
        btn.onClick.AddListener(OnButtonClick);
    }

    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (state.GetSpecialActionWord() || !toggle.isOn) return;
        Wordtext.text = WordSelectedInNotebook.Notebook.GetSelectedWord().GetName();
    }

    public void OnButtonClick()
    {
        toggle.isOn = true;

        if (WordSelectedInNotebook.Notebook.GetSelectedWord())
        {
            Wordtext.text = WordSelectedInNotebook.Notebook.GetSelectedWord().GetName();
        }
        else if (!state.GetSpecialActionWord()) OnShakeNotebook?.Invoke(this, null);

        if (state.GetSpecialActionWord()) Wordtext.text = "";
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
