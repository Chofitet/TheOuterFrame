using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class NotebookWordInstance : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject strikethrough;
    [SerializeField] GameEvent OnWritingShakeNotebook;
    [SerializeField] GameEvent OnCrossWordSound;
    [SerializeField] GameEvent OnWritingNotebookSound;
    [SerializeField] Button btn;
    WordData wordReference;
    bool isCross;
    bool isActiveInBoard;
    public void Initialization(WordData word, bool noAnim = false)
    {
        wordReference = word;
        text.text = wordReference.GetName();
        btn.onClick.AddListener(SetSelectedWord);

        float writingTime = 0;

        if (!word.GetIsFound())
        {
            writingTime = 0.5f;
            word.SetIsFound();
            if (!noAnim) text.gameObject.GetComponent<FadeWordsEffect>().StartEffect();
        }

        if (!noAnim)
        {
            OnWritingShakeNotebook?.Invoke(this, writingTime);
            OnWritingNotebookSound?.Invoke(this, null);
            Invoke("Alpha1", 1);
        }

    }

    public void EraseAnim()
    {
        text.gameObject.GetComponent<FadeWordsEffect>().StartEffect(false);
    }

    public void RefreshWord(Component sender, object obj)
    {
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
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.DOSizeDelta(new Vector2(text.GetComponent<RectTransform>().sizeDelta.x, line.sizeDelta.y), 0.3f);
        OnCrossWordSound?.Invoke(this, null);
        isCross = true;
    }


    public void EraseCrossWord()
    {
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.pivot = new Vector2(1, 0);
        line.localPosition = new Vector2(line.sizeDelta.x, line.localPosition.y);
        line.DOSizeDelta(new Vector2(0, line.sizeDelta.y), 0.3f);
    }

    IEnumerator AnimFade(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        first.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent1);
        yield return new WaitForSeconds(0.5f);
        if (first == second) first.text = txt;
        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
    }

    bool wasSelectedBefore = false;
    public void SetSelectedWord()
    {
        text.text = "<u>" + wordReference.GetName() + "</u>";
        WordSelectedInNotebook.Notebook.SetSelectedWord(wordReference);
        isActiveInBoard = false;
        if (wasSelectedBefore)
        {
            text.text = wordReference.GetName();
            Invoke("ReSelectWord", 0.15f);
        }

        wasSelectedBefore = true;
    }

    public void ClearUnderline()
    {
        if (isActiveInBoard) return;
        text.text = wordReference.GetName();
        Invoke("SetwasSelectedBefore", 0.1f);
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

}

