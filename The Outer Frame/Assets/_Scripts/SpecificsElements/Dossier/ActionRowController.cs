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
    [SerializeField] GameObject DotsLine;
    [SerializeField] TMP_FontAsset writingFont;
    [SerializeField] GameEvent OnWrittingFormSound;
     bool isSpecialAction;
    StateEnum state;
    FadeWordsEffect fade;
    FadeWordsEffect fadeAction;
    bool once;
    WordData Word;

    public void Initialization(StateEnum _state, bool _isFirstTimeIdeaAdded)
    {
        state = _state;
        ActionText.text = _state.GetInfinitiveVerb();
        btn.onClick.AddListener(OnButtonClick);
        fade = Wordtext.GetComponent<FadeWordsEffect>();
        fadeAction = ActionText.GetComponent<FadeWordsEffect>();
        observationTxt.text = state.GetObservationTxt();

        if (_state.GetSpecialActionWord())
        {
            isSpecialAction = true;
            DotsLine.SetActive(false);
            ActionText.font = writingFont;
            Invoke("ClickButton", 2f);
            if (!_isFirstTimeIdeaAdded) return;
            fadeAction.StartEffect();
           
        }
    }

    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (isSpecialAction || !isInView) return;

        Word = WordSelectedInNotebook.Notebook.GetSelectedWord();

        if (toggle.isOn && once)
        {
            StartCoroutine(AnimFade(Wordtext, false, Wordtext, true, Word.GetForm_DatabaseNameVersion()));
            
        }
        if(toggle.isOn && !once)
        {
            Wordtext.text = Word.GetForm_DatabaseNameVersion();
            fade.StartEffect();
            OnWrittingFormSound?.Invoke(this, null);
        }

        if (isSpecialAction || !toggle.isOn || !once)
        {
            once = true;
        }
        
    }

    void ClickButton()
    {
        btn.onClick.Invoke();
    }

    public void OnButtonClick()
    {
        if(!isInView) return;
        toggle.isOn = true;

        if (isSpecialAction) return;
        if (Word)
        {
            Wordtext.text = Word.GetForm_DatabaseNameVersion();
            fade.StartEffect();
            OnWrittingFormSound?.Invoke(this, null);
        }
        else if (!isSpecialAction) OnShakeNotebook?.Invoke(this, null);

        if (isSpecialAction) Wordtext.text = "";
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
        OnWrittingFormSound?.Invoke(this, null);
    }

    bool isInView;

    public void CheckView(Component sender, object obj)
    {
        ViewStates actualView = (ViewStates)obj;

        if (actualView == ViewStates.DossierView || actualView == ViewStates.OnTakenPaperView) isInView = true;
        else isInView = false;
    }

    public void CheckToggle()
    {
        toggle.isOn = true;
    }

    public bool GetIsOn() { return toggle.isOn; }
}
