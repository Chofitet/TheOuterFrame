using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
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
    [SerializeField] GameObject strikethrough;
    [SerializeField] GameEvent OnCrossWordSound;
    NotebookProcessManager processManager;
    WordData word;
    WordData wordNum;
    [SerializeField] Button WordBtn;
    [SerializeField] Button NumBtn;
    [SerializeField] GameEvent OnElementBtn;
    NumberStates ActualState;
    NotebookPhonesController phonesController;
    bool isInWritingFade;

    public void Initialization(WordData _word,NotebookPhonesController _phonesController, NotebookProcessManager _processManager = null)
    {
        if (_processManager != null) processManager = _processManager;

        phonesController = _phonesController;

        WordBtn.GetComponent<OnClickDownEvent>().onPointerDown.AddListener(PressWord);
        NumBtn.GetComponent<OnClickDownEvent>().onPointerDown.AddListener(PressNumber);

        word = WordsManager.WM.FindWordWithPhoneNum_NumberFound(_word);
        if (word.GetIsAPhoneNumber()) word = WordsManager.WM.FindWordWithPhoneNum(_word);
        wordNum = _word;

        if(word.GetIsFound())
        {
            // escribir palabra + num

            phonesController.AddAction(WritingAnim(txtName, word.GetName()));
            phonesController.AddAction(WritingAnim(Num, wordNum.GetName()));
            SetNumState(NumberStates.FoundWithWord);
        }
        else
        {
            // escribir ?? + num
            phonesController.AddAction(WritingAnim(txtName, "?????"));
            phonesController.AddAction(WritingAnim(Num, wordNum.GetName()));
            SetNumState(NumberStates.FoundWithoutWord);
        }

    }

    public void TryUpdateWord(WordData _word)
    {
        if (ActualState == NumberStates.WaitingWord) return;
        SetNumState(NumberStates.WaitingWord);
        lastText = txtName.text;
        word = _word;
    }

    //On
    public void OnSlidePhones(Component sender, object obj)
    {
        if (ActualState == NumberStates.FoundWithWord) ClearWordUnderline(true);
        if (NumBtn.enabled) ClearNumdUnderline(true);
       
    }

    public void OnSlidePhonesUp(Component sender, object obj)
    {
        phonesController.waitSlidePhoneUp = 0.3f;

        if (ActualState == NumberStates.WaitingWord) UpdateWord();

        TryCrossWord();
    }

    float waitSlidePhoneUp = 0;

    string lastText;
    void UpdateWord()
    {
        //remplazar los ??? por la palabra
        phonesController.AddAction(EraseAnim(txtName));
        phonesController.AddAction(WritingAnim(txtName, word.GetName()));
        
        SetNumState(NumberStates.FoundWithWord);

    }

    
    public void TryCrossWord()
    {
        if (ActualState == NumberStates.InactiveWord) return;
        if (word.GetInactiveStateSeen() && !word.GetEraseState())
        {
            
            crossWord();
        }
    }

    void crossWord()
    {
        WordBtn.enabled = false;
        strikethrough.SetActive(true);
        phonesController.AddAction(CrossAnim());
        SetNumState(NumberStates.InactiveWord);
    }

    public void ClearUnderline()
    {
        if (ActualState == NumberStates.FoundWithWord)
        {
            ClearWordUnderline(true);
        }
        if(NumBtn.enabled && !isMovingToPinchofono)
        {
            ClearNumdUnderline(true);
        }
    }


    ViewStates actualView;

    public void CheckView(Component sender, object obj)
    {
       if(GoToPinchofono != null)  StopCoroutine(GoToPinchofono);
        actualView = (ViewStates)obj;
    }

    public WordData GetWord() { return word; }

    public Button GetWordButton() { return WordBtn; }

    public Button GetNumButton() { return NumBtn; }

    bool isMovingToPinchofono;
    public void PressNumber()
    {
        if (actualView != ViewStates.PinchofonoView)
        {
            // tocar num en otra vista te lleva al pinchofono
            OnElementBtn?.Invoke(this, ViewStates.PinchofonoView);
            GoToPinchofono = StartCoroutine(PressNumberBehaviour());
            if (ActualState == NumberStates.FoundWithWord) ClearWordUnderline(true);
             Num.text = $"<u>{wordNum.GetName()}</u>";
            return;
        }
        Num.text = $"<u>{wordNum.GetName()}</u>";
        if(ActualState == NumberStates.FoundWithWord) ClearWordUnderline(true);
        WordSelectedInNotebook.Notebook.SetSelectedWord(wordNum);

        
    }
    Coroutine GoToPinchofono;
    private bool isActiveInBoard;
    private bool isinactive;

    IEnumerator PressNumberBehaviour()
    {
        isMovingToPinchofono = true;
        yield return new WaitForSeconds(0.5f);
        WordSelectedInNotebook.Notebook.SetSelectedWord(wordNum);
        isMovingToPinchofono = false;
    }


    // Writing Anims
   
    IEnumerator WritingAnim(TMP_Text textFild, string text)
    {
        isInWritingFade = true;
        processManager.RegisterProcess();
        OnWritingShakeNotebook?.Invoke(this, 0.3f);
        OnWritingNotebookSound?.Invoke(this, null);
        textFild.text = text;
        //textFild.gameObject.GetComponent<FadeWordsEffect>().SetBlank(0);
        textFild.GetComponent<FadeWordsEffect>().StartEffect();
        yield return new WaitForSeconds(0.3f);
        ResolveStateOnFinish();
        processManager.UnregisterProcess();
    }

    IEnumerator EraseAnim(TMP_Text textFild)
    {
        processManager.RegisterProcess();
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
        txtName.text = lastText;
        var erase = txtName.gameObject.GetComponent<FadeWordsEffect>();
        erase.OnEraseProgress += eraseParticlesName;
        erase.OnComplete += OnEraseFinished;
        erase.StartEffect(false);

        yield return new WaitForSeconds(0.5f);
        processManager.UnregisterProcess();
    }

    public void eraseParticlesName(float progress)
    {
        EraseParticlesName.GetComponent<ParticleSystem>().Play();

        Vector3 initPos = txtName.transform.localPosition;
        EraseParticlesName.localPosition = Vector3.Lerp(initPos, new Vector3(txtName.preferredWidth, 0, 0), progress);
    }

    void OnEraseFinished()
    {
        var fade = txtName.gameObject.GetComponent<FadeWordsEffect>();
        fade.OnComplete -= OnEraseFinished;
        EraseParticlesName.GetComponent<ParticleSystem>().Stop();
        fade.OnEraseProgress -= eraseParticlesName;

    }
    bool isCross;
    IEnumerator CrossAnim()
    {
        if (isCross) yield return null;
       processManager.RegisterProcess();

        
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        line.DOSizeDelta(new Vector2(txtName.GetComponent<RectTransform>().sizeDelta.x, line.sizeDelta.y), 0.3f).OnComplete(() => processManager.UnregisterProcess());
        OnCrossWordSound?.Invoke(this, null);
        isCross = true;

        yield return new WaitForSeconds(0.5f);
        processManager.UnregisterProcess();
    }

    void ResolveStateOnFinish()
    {
        // si hay que tacharla
        if (word.GetInactiveStateSeen())
        {
            crossWord();
            if(actualView != ViewStates.BoardView && actualView != ViewStates.OnTakeSomeInBoard) return;
        }

        //si hay que aplicarle bold por estar en el board
        if (!word.GetPlacedInBoard() & actualView == ViewStates.BoardView || actualView == ViewStates.OnTakeSomeInBoard)
        {
            ApplyMaterial("Board");
            WordBtn.enabled = true;
        }

        isInWritingFade = false;
    }

    public void ReplaceWordInstantly(WordData _word)
    {
        word = _word;
        txtName.text = word.GetName();
    }

    public void ReplaceNumInstantly(WordData _word)
    {
        wordNum = _word;
        Num.text = word.GetName();
    }

    void SetNumState(NumberStates newState)
    {
        switch(newState)
        {
            case NumberStates.NoFound:
                break;
            case NumberStates.NoFoundWithWord:
                WordBtn.enabled = false;
                NumBtn.enabled = true;
                break;
            case NumberStates.FoundWithoutWord:
                WordBtn.enabled = false;
                NumBtn.enabled = true;
                break;
            case NumberStates.WaitingWord:
                WordBtn.enabled = false;
                NumBtn.enabled = false;
                break;
            case NumberStates.FoundWithWord:
                WordBtn.enabled = true;
                NumBtn.enabled = true;
                break;
            case NumberStates.InactiveWord:
                WordBtn.enabled = false;
                NumBtn.enabled = true;
                break;
        }
        ActualState = newState;
    }


    #region WordPressLogic

    public void PressWord()
    {
        txtName.text = "<u>" + word.GetName() + "</u>";
        if (actualView == ViewStates.BoardView) txtName.text = word.GetName();
        WordSelectedInNotebook.Notebook.SetSelectedWord(word);
        if (NumBtn.enabled) ClearNumdUnderline(true);
        isActiveInBoard = false;
        WordBtn.enabled = false;
        if (actualView == ViewStates.PCView)
        {
            Invoke("UnSelectWord", 0.3f);
        }
        if (actualView == ViewStates.OnTakeSomeInBoard) OnElementBtn?.Invoke(this, ViewStates.BoardView);
    }
    void UnSelectWord()
    {
        txtName.text = word.GetName();
        WordBtn.enabled = true;
    }

    public void ClearWordUnderline(bool clearDirectly = false)
    {
        if(!clearDirectly) if (WordSelectedInNotebook.Notebook.GetSelectedWord() == word) return;
        if (isActiveInBoard) return;
        if (isinactive) return;
        WordBtn.enabled = true;
        if(isInWritingFade) return; 
        txtName.text = word.GetName();
    }
    public void ClearNumdUnderline( bool clearDirectly = false)
    {
        if (!clearDirectly) if (WordSelectedInNotebook.Notebook.GetSelectedWord() == wordNum) return;
        NumBtn.enabled = true;
        Num.text = wordNum.GetName();
    }

   
    public void TryActiveWord(bool x,string isBoard = "")
    {
        if (ActualState == NumberStates.FoundWithWord)
        {
            WordBtn.enabled = x;
        }

        if(ActualState == NumberStates.InactiveWord)
        {
           WordBtn.enabled = x;
        }

        if(ActualState == NumberStates.InactiveWord && isBoard != "Board")
        {
            WordBtn.enabled = false;
        }
        
        NumBtn.enabled = true;

        if (word.GetPhoneNumber() == "UNLISTED")
        {
            WordBtn.enabled = false;
            NumBtn.enabled = false;
        }
    }

    public void SetInactive(bool x) => isinactive = x;

    #endregion

    #region Board Material

    string materialName;
    public void ApplyMaterial(string materialLabel = "")
    {
        if (txtName.text.Contains("<material=")) return;

        materialName = "\"" + txtName.font.name + "" + materialLabel;

        materialName = materialName.Replace(" ", "");

        string newWord = "<material=" + materialName + ">" + txtName.text + "</material>";

        txtName.text = newWord;

    }

    private Sequence thicknessSequence;

    /// material modificator

    public void InactiveDirectly()
    {
        thicknessSequence?.Kill();

        txtName.text = word.GetName();

        WordBtn.enabled = false;
    }

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
        if (materialName == null) return txtName.GetComponent<ShaderMaterialManager>().GetFirstMat();
        return txtName.GetComponent<ShaderMaterialManager>().GetHighLigthMaterial(materialName.Replace("\"", ""));
    }

    #endregion

}

public enum NumberStates
{
    NoFound,
    NoFoundWithWord,
    FoundWithoutWord,
    WaitingWord,
    FoundWithWord,
    WaitingCross,
    InactiveWord,
}