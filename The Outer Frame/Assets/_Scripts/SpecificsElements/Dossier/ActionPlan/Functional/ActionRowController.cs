using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.ParticleSystem;

public class ActionRowController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtext;
    [SerializeField] GameObject strikethrough;
    [SerializeField] TMP_Text ActionText;
    [SerializeField] Toggle toggle;
    [SerializeField] Button btn;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] TMP_Text observationTxt;
    [SerializeField] TMP_Text AclarationTxt;
    [SerializeField] GameObject DotsLine;
    [SerializeField] TMP_FontAsset writingFont;
    [SerializeField] GameEvent OnWrittingFormSound;
    [SerializeField] Transform EraseParticles;
    [SerializeField] Transform EraseParticlesAclaration;
    
     bool isSpecialAction;
    StateEnum state;
    FadeWordsEffect fade;
    FadeWordsEffect fadeAction;
    FadeWordsEffect fadeAclaration;
    bool once;
    WordData Word;

    public void Initialization(StateEnum _state, bool _isFirstTimeIdeaAdded)
    {
        state = _state;
        ActionText.text = _state.GetInfinitiveVerb();
        btn.onClick.AddListener(OnButtonClick);
        fade = Wordtext.GetComponent<FadeWordsEffect>();
        fadeAction = ActionText.GetComponent<FadeWordsEffect>();
        fadeAclaration = AclarationTxt.GetComponent<FadeWordsEffect>();
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
            StartCoroutine(AnimEraseWriting(Wordtext, false, Wordtext, true, Word.GetFormNameVersion()));
            
        }
        if(toggle.isOn && !once)
        {
            Wordtext.text = Word.GetFormNameVersion();
            StartCoroutine(AnimOnlyWriting(Wordtext, true));
        }

        if (isSpecialAction || !toggle.isOn || !once)
        {
            once = true;
        }
        
    }

    void ClickButton()
    {
        if (btn == null) return;
        btn.onClick.Invoke();
    }

    public void OnButtonClick()
    {
        if(!isInView) return;

        btn.enabled = false;
        Invoke("ReacticeBTN",0.07f);
        if (isSpecialAction)
        {
            if (!toggle.isOn) Invoke("CheckToggle", 0.01f);
            else Invoke("UnCheckToggle", 0.01f);
            return;
        }

        if (!toggle.isOn) buttonCheck();
        else buttonUncheck();

    }

    void ReacticeBTN()
    {
        btn.enabled = true;
    }

    void buttonCheck()
    {
        Invoke("CheckToggle", 0.01f);
        if (Word)
        {
            Wordtext.text = Word.GetFormNameVersion();
            StartCoroutine(AnimOnlyWriting(Wordtext, true));
        }
        else if (!isSpecialAction) OnShakeNotebook?.Invoke(this, null);

        if (isSpecialAction) Wordtext.text = "";
    }

    void buttonUncheck()
    {
        Invoke("UnCheckToggle", 0.01f);
    }

    public Button GetButton() { return btn; }

    public StateEnum GetState() { return state; }

    public void ResetRow()
    {
        toggle.isOn = false;
        if (!fade.GetisVisible()) return;
        StartCoroutine(AnimOnlyErase());
        
    }
    void Erasefinished()
    {
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        fade.OnEraseProgress -= eraseParticles;
    }

    public void DesactiveRow()
    {
        btn.enabled = false;
        strikethrough.SetActive(true);
    }

    IEnumerator AnimEraseWriting(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        FadeWordsEffect effect = first.gameObject.GetComponent<FadeWordsEffect>();
        if (!isTransparent1) effect.OnEraseProgress += eraseParticles;
        effect.StartEffect(isTransparent1);
        yield return new WaitForSeconds(0.2f);
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        fade.OnEraseProgress -= eraseParticles;

        if (AclarationTxt.text != string.Empty)
        {
            fadeAclaration.OnEraseProgress += eraseParticlesAclatarion;
            fadeAclaration.StartEffect(false);
            yield return new WaitForSeconds(0.2f);
            EraseParticlesAclaration.GetComponent<ParticleSystem>().Stop();
            fadeAclaration.OnEraseProgress -= eraseParticlesAclatarion;
            AclarationTxt.text = "";
        }

        if (first == second) first.text = txt;

        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
        OnWrittingFormSound?.Invoke(this, null);
        yield return new WaitForSeconds(0.2f);
        if (Word.GetWordFirstLocationAppear() != string.Empty && state.GetNeedWordLocation())
        {
            AnimAclarationText(Word.GetWordFirstLocationAppear());
            OnWrittingFormSound?.Invoke(this, null);
        }
    }

    IEnumerator AnimOnlyWriting(TMP_Text text, bool isTransparent1)
    {
        FadeWordsEffect effect = text.gameObject.GetComponent<FadeWordsEffect>();
        effect.StartEffect(isTransparent1);
        OnWrittingFormSound?.Invoke(this, null);
        yield return new WaitForSeconds(0.2f);
        if (Word.GetWordFirstLocationAppear() != string.Empty && state.GetNeedWordLocation())
        {
            AnimAclarationText(Word.GetWordFirstLocationAppear());
            OnWrittingFormSound?.Invoke(this, null);
        }
    }

    IEnumerator AnimOnlyErase()
    {
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        fade.StopAllCoroutines();
        fade.StartEffect(false);
        fade.OnEraseProgress += eraseParticles;
        fade.OnComplete += Erasefinished;
        if (Word.GetWordFirstLocationAppear() != string.Empty && state.GetNeedWordLocation())
        {
            yield return new WaitForSeconds(0.2f);
            fadeAclaration.OnEraseProgress += eraseParticlesAclatarion;
            fadeAclaration.StartEffect(false);
            yield return new WaitForSeconds(0.2f);
            EraseParticlesAclaration.GetComponent<ParticleSystem>().Stop();
            fadeAclaration.OnEraseProgress -= eraseParticlesAclatarion;
            AclarationTxt.text = "";
        }
    }

    public void eraseParticles(float progress)
    {
        EraseParticles.GetComponent<ParticleSystem>().Play();
        Vector3 initPos = Wordtext.transform.localPosition + new Vector3(2,0,0);
        EraseParticles.localPosition = Vector3.Lerp(initPos, new Vector3(Wordtext.transform.localPosition.x + Wordtext.preferredWidth, initPos.y, initPos.z), progress);
    }

    public void eraseParticlesAclatarion(float progress)
    {
        EraseParticlesAclaration.GetComponent<ParticleSystem>().Play();
        Vector3 initPos = AclarationTxt.transform.localPosition;
        EraseParticlesAclaration.localPosition = Vector3.Lerp(initPos, new Vector3(0, initPos.y, initPos.z), progress);
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
    public void UnCheckToggle()
    {
        toggle.isOn = false;
    }

    void AnimAclarationText(string text)
    {
        AclarationTxt.transform.localPosition = Vector3.zero;
        AclarationTxt.text = text;
        AclarationTxt.ForceMeshUpdate();
        float textleght = AclarationTxt.textBounds.size.x;
        AclarationTxt.transform.localPosition = new Vector3(-textleght, 0, 0);

        fadeAclaration.StartEffect();
    }


    public bool GetIsOn() { return toggle.isOn; }
}
