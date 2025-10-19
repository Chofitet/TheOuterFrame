using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneRowNotebookController : MonoBehaviour
{
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text Num;
    [SerializeField] GameEvent OnWritingShakeNotebook;
    [SerializeField] GameEvent OnWritingNotebookSound;
    [SerializeField] Transform EraseParticlesName;
    [SerializeField] Transform EraseParticlesNum;
    NotebookProcessManager processManager;
    WordData word;
    Button button;

    public void Initialization(WordData _word, bool NoAnim = false, NotebookProcessManager _processManager = null)
    {
        if (_processManager != null) processManager = _processManager;
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonPress);
        word = _word;
        txtName.text = word.GetName();
        float writingTime = 0;

        if (word.GetIsAPhoneNumber())
        {
            writingTime = 0.5f;
            txtName.text = WordsManager.WM.FindWordWithPhoneNum(word).GetName(); // volver a los "?????"
            Num.text = word.GetPhoneNumber();
            button.enabled = true;
            if (!NoAnim)
            {
                //Num.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
                //txtName.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);

                StartCoroutine(AnimFade(txtName, true, Num, true));
            }
        }

        if (!word.GetIsPhoneNumberFound())
        {
            writingTime = 0.5f + 0.5f;
            Num.text = "?????";
            if (!NoAnim)
            {
                Num.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
                txtName.gameObject.GetComponent<FadeWordsEffect>().StartEffect(true);
            }
        }

        if (!NoAnim)
        {
            OnWritingShakeNotebook?.Invoke(this, writingTime);
            OnWritingNotebookSound?.Invoke(this, null);
        }

    }

    public void UpdateNumber()
    {
        button.enabled = true;
        Num.text = "?????";
        StartCoroutine(AnimFade(Num, false, Num, true, word.GetPhoneNumber()));
    }

    public void ReplaceNumberWithWord(WordData _word)
    {
        word = _word;
        txtName.text = "?????"; // volver a los "?????"
        StartCoroutine(AnimFade(txtName, false, txtName, true, word.GetName()));
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
    }

    public void ReplaceNumber(WordData _word)
    {
        txtName.text = word.GetName();
        word = _word;
        StartCoroutine(AnimFade(txtName, false, txtName, true, _word.GetName()));

        if (_word.GetIsPhoneNumberFound())
        {
            string auxNum = "?????"; // volver a los "?????"
            auxNum = _word.GetPhoneNumber();

            StartCoroutine(AnimFade(Num, false, Num, true, auxNum));
        }
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
    }

    public void ReplaceWordInstantly(WordData _word)
    {
        word = _word;
        txtName.text = word.GetName();
    }

    public void EraseAnim()
    {
        StartCoroutine(AnimFade(txtName, false, txtName, true, " "));

        StartCoroutine(AnimFade(Num, false, Num, true, " "));
    }

    private void ButtonPress()
    {
        if (!button.interactable) return;

        if (word.GetIsAPhoneNumber() && actualView != ViewStates.PinchofonoView)
        {
            return;
        }

        if (word.GetIsAPhoneNumber())
        {
            Num.text = "<u>" + word.GetPhoneNumber() + "</u>";
            WordSelectedInNotebook.Notebook.SetSelectedWord(word);
            button.enabled = false;
            return;
        }

        if (actualView == ViewStates.PinchofonoView)
        {
            if (!word.GetIsPhoneNumberFound()) return;
            Num.text = "<u>" + word.GetPhoneNumber() + "</u>";
            WordSelectedInNotebook.Notebook.SetSelectedWord(word);
            button.enabled = false;
            return;
        }
        else
        {
            txtName.text = "<u>" + word.GetName() + "</u>";

            WordSelectedInNotebook.Notebook.SetSelectedWord(word);
            button.enabled = false;
        }


    }



    public void ClearUnderline()
    {
        if (word.GetIsAPhoneNumber())
        {
            Num.text = word.GetName();
            button.enabled = true;
            return;
        }
        if (word.GetIsPhoneNumberFound()) Num.text = word.GetPhoneNumber();
        txtName.text = word.GetName();
    }

    ViewStates actualView;

    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
    }

    IEnumerator AnimFade(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        processManager.RegisterProcess();
        second.gameObject.GetComponent<FadeWordsEffect>().SetBlank(0);
        first.gameObject.GetComponent<FadeWordsEffect>().SetBlank(0);
        first.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent1);

        yield return new WaitForSeconds(0.5f);
        if (first == second) first.text = txt;
        second.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
        OnWritingNotebookSound?.Invoke(this, null);
        yield return new WaitForSeconds(0.5f);
        processManager.UnregisterProcess();
    }

    public void eraseParticlesName(float progress)
    {
        EraseParticlesName.GetComponent<ParticleSystem>().Play();

        Vector3 initPos = txtName.transform.localPosition;
        EraseParticlesName.localPosition = Vector3.Lerp(initPos, new Vector3(txtName.preferredWidth, initPos.y, initPos.z), progress);
    }
    public void eraseParticlesNum(float progress)
    {
        EraseParticlesNum.GetComponent<ParticleSystem>().Play();

        Vector3 initPos = Num.transform.localPosition;
        EraseParticlesNum.localPosition = Vector3.Lerp(initPos, new Vector3(Num.preferredWidth, initPos.y, initPos.z), progress);
    }

    public WordData GetWord() { return word; }

    public Button GetButton() { return button; }
}
