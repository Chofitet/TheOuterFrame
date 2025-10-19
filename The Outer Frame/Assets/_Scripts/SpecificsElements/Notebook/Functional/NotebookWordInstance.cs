using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using static System.Net.Mime.MediaTypeNames;

public class NotebookWordInstance : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject strikethrough;
    [SerializeField] GameEvent OnWritingShakeNotebook;
    [SerializeField] GameEvent OnCrossWordSound;
    [SerializeField] GameEvent OnWritingNotebookSound;
    [SerializeField] Button btn;
    [SerializeField] Transform EraseParticles;
    NotebookProcessManager processManager;
    WordData wordReference;
    bool isCross;
    bool isActiveInBoard;
    bool isinactive;
    public void Initialization(WordData word, bool noAnim = false, NotebookProcessManager _processManager = null)
    {
        if(_processManager != null) processManager = _processManager;
        wordReference = word;
        text.text = wordReference.GetName();
        btn.onClick.AddListener(SetSelectedWord);

        float writingTime = 0;

        if (!word.GetIsFound())
        {
            writingTime = 0.5f;
            word.SetIsFound();
            if (!noAnim)
            {
                processManager.RegisterProcess();
                var fade = text.gameObject.GetComponent<FadeWordsEffect>();
                fade.OnComplete += OnWritingFinished;
                fade.StartEffect();
            }
        }

        if (!noAnim)
        {
            OnWritingShakeNotebook?.Invoke(this, writingTime);
            OnWritingNotebookSound?.Invoke(this, null);
            Invoke("Alpha1", 1);
        }

    }

    void OnWritingFinished()
    {
        var fade = text.gameObject.GetComponent<FadeWordsEffect>();
        fade.OnComplete -= OnWritingFinished;

        processManager.UnregisterProcess();
        Debug.Log($"writing of {wordReference.GetName()} finished");
    }

    public void EraseAnim()
    {
        processManager.RegisterProcess();
        var erase = GetComponent<FadeWordsEffect>();
        erase.OnEraseProgress += eraseParticles;
        erase.OnComplete += OnEraseFinished;
        erase.StartEffect(false);
    }
    public void eraseParticles(float progress)
    {
        EraseParticles.GetComponent<ParticleSystem>().Play();
        Debug.Log("erase progress" + progress.ToString());
        EraseParticles.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(text.preferredWidth,0,0), progress);
    }
    void OnEraseFinished()
    {
        var fade = text.gameObject.GetComponent<FadeWordsEffect>();
        fade.OnComplete -= OnEraseFinished;
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        fade.OnEraseProgress -= eraseParticles;

        processManager.UnregisterProcess();
        Debug.Log($"Erase of {wordReference.GetName()} finished");
    }

    public void RefreshWord(Component sender, object obj)
    {
        if (wordReference == null) return;
        if (wordReference.GetInactiveState() && !wordReference.GetEraseState())
        {
            btn.enabled = false;
            strikethrough.SetActive(true);
            CrossOutWord();
        }

    }
    public void ReplaceWord(WordData word)
    {
        Debug.Log(wordReference.GetName());
        text.text = wordReference.GetName();
        if (isCross) EraseCrossWord();
        StartCoroutine(AnimFade(text, false, text, true, word.GetName()));
        wordReference = word;
        word.SetIsFound();
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
        btn.enabled = true;
    }

    public void ReplaceWordInstantly(WordData word)
    {
        wordReference = word;
        word.SetIsFound();
        text.text = wordReference.GetName();

    }

    public void CrossOutWord()
    {
        if (isCross) return;
        processManager.RegisterProcess();
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.DOSizeDelta(new Vector2(text.GetComponent<RectTransform>().sizeDelta.x, line.sizeDelta.y), 0.3f).OnComplete(() => processManager.UnregisterProcess());
        OnCrossWordSound?.Invoke(this, null);
        isCross = true;
    }


    public void EraseCrossWord()
    {
        processManager.RegisterProcess();
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.pivot = new Vector2(1, 0);
        line.localPosition = new Vector2(line.sizeDelta.x, line.localPosition.y);
        line.DOSizeDelta(new Vector2(0, line.sizeDelta.y), 0.3f).OnComplete(() => processManager.UnregisterProcess());
    }

    IEnumerator AnimFade(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        processManager.RegisterProcess();
        FadeWordsEffect effect = first.gameObject.GetComponent<FadeWordsEffect>();
        effect.StartEffect(isTransparent1);
        if (!isTransparent1) effect.OnEraseProgress += eraseParticles;
        yield return new WaitForSeconds(0.5f);
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        if (first == second) first.text = txt;
        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
        yield return new WaitForSeconds(0.5f);
        processManager.UnregisterProcess();
    }

    bool wasSelectedBefore = false;
    public void SetSelectedWord()
    {
       text.text = "<u>" + wordReference.GetName() + "</u>";
        if (actualView == ViewStates.BoardView) text.text = wordReference.GetName();
        WordSelectedInNotebook.Notebook.SetSelectedWord(wordReference);
        isActiveInBoard = false;
        btn.enabled = false;
        /*if (wasSelectedBefore)
        {
            text.text = wordReference.GetName();
            Invoke("ReSelectWord", 0.15f);
        }

        wasSelectedBefore = true;*/
    }

    public void ClearUnderline()
    {
        if (isActiveInBoard) return;
        if (isinactive) return;
        btn.enabled = true;
        text.text = wordReference.GetName();
        //Invoke("SetwasSelectedBefore", 0.1f);
    }

    void Alpha1()
    {
        text.color = new Vector4(text.color.r, text.color.g, text.color.b, 1);
    }

    public WordData GetWord() { return wordReference; }

    void ReSelectWord()
    {
        text.text = "<u>" + wordReference.GetName() + "</u>";
    }
    void SetwasSelectedBefore()
    {
        wasSelectedBefore = false;
    }

    public Button GetButton()
    {
        return btn;
    }
    string materialName;
    public void ApplyMaterial(string materialLabel = "")
    {
        if (text.text.Contains("<material=")) return;

        materialName = "\"" + text.font.name + "" + materialLabel;

        materialName = materialName.Replace(" ", "");

        string newWord = "<material=" + materialName + ">" + text.text + "</material>";

        text.text = newWord;

    }

    private Sequence thicknessSequence;

    /// material modificator

    public void ApplyThicknessAnim(bool x)
    {
        if (x)
        {
            ThicknessOn();
            isActiveInBoard = true;
        }
        else
        {
            ThicknessOff();
            isActiveInBoard = false;
            
        }
    }

    public void ThicknessOn(float targetValue = 0.3f, float duration = 0.3f)
    {
        Material mat = GetMat();

        // Cancelamos cualquier animación previa
        thicknessSequence?.Kill();

        thicknessSequence = DOTween.Sequence();

        thicknessSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_FaceDilate),      
                x => mat.SetFloat(ShaderUtilities.ID_FaceDilate, x),   // setter
                targetValue,                                           // valor final
                duration                                               // duración
            ).SetEase(Ease.InOutSine)
        );
    }

    public void ThicknessOff(float endValue = 0f, float duration = 0.3f)
    {
        Material mat = GetMat();

        thicknessSequence?.Kill();

        thicknessSequence = DOTween.Sequence();

        thicknessSequence.Append(
            DOTween.To(
                () => mat.GetFloat(ShaderUtilities.ID_FaceDilate),
                x => mat.SetFloat(ShaderUtilities.ID_FaceDilate, x),
                endValue,
                duration
            ).SetEase(Ease.InOutSine)
            
        );
    }

    private Material GetMat()
    {
        // Igual que en tu ejemplo, usás tu propio manager de materiales
        return text.GetComponent<ShaderMaterialManager>().GetHighLigthMaterial(materialName.Replace("\"", ""));
    }

    ViewStates actualView;
    public void CheckView(Component sender,object obj)
    {
        actualView = (ViewStates)obj;
    }

    public void SetInactive(bool x) => isinactive = x;
}

