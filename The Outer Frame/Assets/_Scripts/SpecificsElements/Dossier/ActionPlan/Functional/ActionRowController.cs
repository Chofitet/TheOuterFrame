using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ActionRowController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtext;
    [SerializeField] GameObject strikethrough;
    [SerializeField] TMP_Text ActionText;
    [SerializeField] Toggle toggle;
    [SerializeField] Image toggleImage;
    [SerializeField] Sprite handmadeToggle;
    [SerializeField] Button btn;
    [SerializeField] GameEvent OnShakeNotebook;
    [SerializeField] TMP_Text observationTxt;
    [SerializeField] TMP_Text AclarationTxt;
    [SerializeField] GameObject DotsLine;
    [SerializeField] TMP_FontAsset writingFont;
    [SerializeField] GameEvent OnWrittingFormSound;
    [SerializeField] Transform EraseParticles;
    [SerializeField] Transform EraseParticlesAclaration;
    [SerializeField] GameEvent OnForceSelectedWordInActionRows; 
    
     bool isSpecialAction;
    StateEnum state;
    FadeWordsEffect fade;
    FadeWordsEffect fadeAction;
    FadeWordsEffect fadeAclaration;
    bool once;
    WordData Word;
    bool isPendigToErase;
    Coroutine EraseAndWriteWord;

    public void Initialization(StateEnum _state, bool _isFirstTimeIdeaAdded)
    {
        state = _state;
        ActionText.text = _state.GetInfinitiveVerb();
        btn.GetComponent<OnClickDownEvent>().onPointerDown.AddListener(() => OnButtonClick(false));
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
            fade = fadeAction;
            toggleImage.sprite = handmadeToggle;
            if (!_isFirstTimeIdeaAdded) return;
            fadeAction.StartEffect();
        }
    }

    WordData WordPendingToReplaceErased;
    bool ApTakedByPressAWord;
    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (isSpecialAction || !isInView) return;

        ApTakedByPressAWord = true;

        if (obj != null) Word = (WordData)obj;
        else Word = WordSelectedInNotebook.Notebook.GetSelectedWord();

        if (isPendigToErase)
        {
            if (ActionText.text == "Inspect") WordPendingToReplaceErased = WordSelectedInNotebook.Notebook.GetSelectedWord();
            else Word = WordSelectedInNotebook.Notebook.GetSelectedWord();
            return;
        }

        

        if (toggle.isOn && once)
        {
            EraseAndWriteWord = StartCoroutine(AnimEraseWriting(Wordtext, false, Wordtext, true, Word.GetFormNameVersion()));
            
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
        btn.GetComponent<OnClickDownEvent>().onPointerDown.Invoke();
    }

    public void OnButtonClick(bool isAutomatic = false)
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
        else if (isAutomatic) buttonUncheck();

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
            once = true;
        }
        else if (!isSpecialAction) OnShakeNotebook?.Invoke(this, null);

        if (isSpecialAction) Wordtext.text = "";
    }

    void buttonUncheck()
    {
        Invoke("UnCheckToggle", 0.01f);
    }

    public void OnForceChangeWord(Component sneder,object obj)
    {
       if(!isSpecialAction) Word = (WordData)obj;
    }

    public Button GetButton() { return btn; }

    public StateEnum GetState() { return state; }

    public void ResetRow(bool noPlaySound = false, bool CancelPending = true)
    {
        toggle.isOn = false;
        once = false;
        if (isSpecialAction) return;
        if (!fade.GetisVisible()) return;
        StartCoroutine(AnimOnlyErase());
       if(EraseAndWriteWord != null)
        {
            StopCoroutine(EraseAndWriteWord);
            EraseAndWriteWord = null;
            Wordtext.text = "";
        }

    }

    public void ResetActionRow()
    {
        toggle.isOn = false;
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
        if (!Word) yield break;
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
        if (Word)
        {
            if (Word.GetWordFirstLocationAppear() != string.Empty && state.GetNeedWordLocation() && !WordPendingToReplaceErased)
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
        if(WordPendingToReplaceErased)
        {
            yield return new WaitForSeconds(0.4f);
            if (!WordPendingToReplaceErased) yield break;
            toggle.isOn = true;
            OnSelectWordInNotebook(null, WordPendingToReplaceErased);
            OnForceSelectedWordInActionRows?.Invoke(this, WordPendingToReplaceErased);
            WordPendingToReplaceErased = null;
            
        }
    }

    public void eraseParticles(float progress)
    {
        TMP_Text actualField = Word ? Wordtext : ActionText;

        if(!Word && isSpecialAction) toggleImage.enabled = false;

        if (actualField == null) return;

        actualField.ForceMeshUpdate();
        var textInfo = actualField.textInfo;

        int lineCount = textInfo.lineCount;
        if (lineCount == 0) return;

        EraseParticles.GetComponent<ParticleSystem>().Play();

        float totalWidth = 0f;
        float[] lineWidths = new float[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            var line = textInfo.lineInfo[i];

            if (line.firstCharacterIndex > line.lastCharacterIndex)
            {
                lineWidths[i] = 0;
                continue;
            }

            var firstChar = textInfo.characterInfo[line.firstCharacterIndex];
            var lastChar = textInfo.characterInfo[line.lastCharacterIndex];

            float width = lastChar.topRight.x - firstChar.bottomLeft.x;
            lineWidths[i] = width;
            totalWidth += width;
        }

        if (totalWidth <= 0f) return;

        float accumulated = 0f;

        for (int i = 0; i < lineCount; i++)
        {
            float normalized = lineWidths[i] / totalWidth;

            if (progress <= accumulated + normalized)
            {
                var line = textInfo.lineInfo[i];

                if (line.firstCharacterIndex > line.lastCharacterIndex) return;

                var firstChar = textInfo.characterInfo[line.firstCharacterIndex];
                var lastChar = textInfo.characterInfo[line.lastCharacterIndex];

                float localProgress = (progress - accumulated) / normalized;

                float startX = firstChar.bottomLeft.x;
                float endX = lastChar.topRight.x;
                float y = firstChar.bottomLeft.y;

                float x = Mathf.Lerp(startX, endX, localProgress);

                Vector3 offset = actualField.transform.localPosition + new Vector3(2, 0, 0);
                EraseParticles.localPosition = new Vector3(x, y, 0) + offset;

                return;
            }

            accumulated += normalized;
        }
    }

    void EraseLine()
    {

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

    Coroutine PendingToEraseCoroutine;

    public void OnInactiveReplaceWord(Component SENDER, object obj)
    {
        WordData _word = (WordData) obj;
        if (!Word) return;

        Debug.Log($"Erase word {_word} from AP");

        if(_word == Word)
        {
            if (toggle.isOn) isPendigToErase = true;
        }
    }

    public void OnAPView(Component sender, object obj)
    {
        bool isInAp = (bool)obj;

        ApTakedByPressAWord = false;

        if (!isPendigToErase) return;
        

        if (isInAp)
        {
            PendingToEraseCoroutine = StartCoroutine(TriggerPendingToEraseCoroutine());
        }
        else
        {
            if(PendingToEraseCoroutine != null) StopCoroutine(PendingToEraseCoroutine);
        }

    }


    IEnumerator TriggerPendingToEraseCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (!ApTakedByPressAWord && !Word.GetInactiveState())
        {
            isPendigToErase = false;
            WordData newWord = WordsManager.WM.FindActualWordRetroactive(Word);
            WordSelectedInNotebook.Notebook.ForceSelectedWord(newWord);
            OnSelectWordInNotebook(null, newWord);
            OnForceSelectedWordInActionRows?.Invoke(this, newWord);

        }
        else
        {
            ResetRow();
            OnForceSelectedWordInActionRows?.Invoke(this, null);
            isPendigToErase = false;
        }
        
    }

    public bool GetIsOn() { return toggle.isOn; }

    public bool GetIsAnSpecialAction() { return isSpecialAction; }
}
