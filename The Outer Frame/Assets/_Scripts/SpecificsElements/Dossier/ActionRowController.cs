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
    [SerializeField] TMP_Text observationTxt;
    StateEnum state;
    FadeWordsEffect fade;
    bool once;

    public void Initialization(StateEnum _state)
    {
        state = _state;
        ActionText.text = _state.GetInfinitiveVerb();
        btn.onClick.AddListener(OnButtonClick);
        fade = Wordtext.GetComponent<FadeWordsEffect>();
        observationTxt.text = state.GetObservationTxt();
    }

    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (toggle.isOn && once)
        {
            StartCoroutine(AnimFade(Wordtext, false, Wordtext, true, WordSelectedInNotebook.Notebook.GetSelectedWord().GetName()));
        }
        if(toggle.isOn && !once)
        {
            Wordtext.text = WordSelectedInNotebook.Notebook.GetSelectedWord().GetName();
            fade.StartEffect();
        }

        if (state.GetSpecialActionWord() || !toggle.isOn || !once)
        {
            once = true;
        }
        
       
    }

    public void OnButtonClick()
    {
        toggle.isOn = true;

        if (WordSelectedInNotebook.Notebook.GetSelectedWord())
        {
            Wordtext.text = WordSelectedInNotebook.Notebook.GetSelectedWord().GetName();
            fade.StartEffect();
        }
        else if (!state.GetSpecialActionWord()) OnShakeNotebook?.Invoke(this, null);

        if (state.GetSpecialActionWord()) Wordtext.text = "";
    }

    public Button GetButton() { return btn; }

    public StateEnum GetState() { return state; }

    public void ResetRow()
    {
        if (!toggle.isOn) return;
        toggle.isOn = false;
        fade.StopAllCoroutines();
        fade.StartEffect(false);
    }

    public void DesactiveRow()
    {
        btn.enabled = false;
        strikethrough.SetActive(true);
    }

    IEnumerator AnimFade(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        first.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent1);
        yield return new WaitForSeconds(0.2f);
        if (first == second) first.text = txt;
        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
    }
}
