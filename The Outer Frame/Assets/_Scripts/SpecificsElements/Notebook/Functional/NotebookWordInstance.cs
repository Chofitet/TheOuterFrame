using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;

public class NotebookWordInstance : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] private int minCharacters = 5;
    [SerializeField] private float CharactersOffset = 2f;
    [SerializeField] float MinWidthToIndent;
    [SerializeField] GameObject strikethrough;
    [SerializeField] FadeWordsEffect fade;
    [SerializeField] GameEvent OnWritingShakeNotebook;
    [SerializeField] GameEvent OnCrossWordSound;
    [SerializeField] GameEvent OnWritingNotebookSound;
    [SerializeField] GameEvent OnWritingWordFinished;
    [SerializeField] Button btn;
    [SerializeField] Transform EraseParticles;
    [SerializeField] GameEvent OnButtonElement;
    [SerializeField] GameEvent OnInactiveReplaceWord;
    [SerializeField] GameEvent OnRequestChangePage;
    [SerializeField] GameEvent OnSendReportAutomatly;

    NotebookProcessManager processManager;
    WordData wordReference;
    float PassPageTime;
    bool isCross;
    bool isActiveInBoard;
    bool isinactive;
    bool PendingToAddBoard;
    float waitAtTheEnd = 0;
    int pageNum;
    int actualPage;
    bool isInSecondColumn;
    bool isInWritingFade;
    NotebookPassPages notebookPasspage;
    
    public void Initialization(WordData word, int _pageNum, NotebookPassPages _notebookPasspage,float _PassPageTime, ViewStates _actualView, bool noAnim = false, NotebookProcessManager _processManager = null, float height = 0, float extraTimeWhilePassingPage = 0)
    {
        if(_processManager != null) processManager = _processManager;
        wordReference = word;
        text.text = wordReference.GetName();
        btn.GetComponent<OnClickDownEvent>().onPointerDown.AddListener(SetSelectedWord);


        actualView = _actualView;
        pageNum = _pageNum;
        notebookPasspage = _notebookPasspage;
        PassPageTime = _PassPageTime;

        if (isInSecondColumn) indentWord();

        if (!word.GetIsFound())
        {
            word.SetIsFound();
            if (!noAnim)
            {
                WriteFade();
            }
        }
        else takeDossierWithWordOnBeginningOnce = true;

        if (word.GetWordWasRemember()) takeDossierWithWordOnBeginningOnce = true;

        if (height != 0)
        {
            RectTransform rt = transform.GetChild(0).GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            size.y = height;
            rt.sizeDelta = size;
        }

    }

    void indentWord()
    {
        text.ForceMeshUpdate();

        if(text.text =="President Deandra")
        {
            Debug.Log("a");
        }
        RectTransform rt = GetComponent<RectTransform>();

        TMP_CharacterInfo[] textInfo = text.textInfo.characterInfo;

        float wordWidht = 0;
        float wordExtraWidht = 0;

        foreach(TMP_CharacterInfo character in textInfo)
        {
            float characterWidth = Math.Abs(character.bottomLeft.x - character.bottomRight.x);

            wordWidht += characterWidth;

            if(wordWidht > MinWidthToIndent) wordExtraWidht += characterWidth;
        }

        rt.anchoredPosition += Vector2.left * (wordExtraWidht + 0.6f);

    }

    public void IsInSecondColumn()
    {
        isInSecondColumn = true;
    }

    public async Task WriteFade()
    {
        processManager.RegisterProcess();
        fade.OnComplete += OnWritingFinished;
        fade.SetBlank(0);
        isInWritingFade = true;

        /*if (extraTimeWhilePassingPage != 0) extraTimeWhilePassingPage = extraTimeWhilePassingPage + 0.5f;
        yield return new WaitForSeconds(extraTimeWhilePassingPage);*/

        /*if (needPassPage)
        {
            OnRequestChangePage?.Invoke(this, pageNum);
            yield return new WaitForSeconds(PassPageTime); // wait for pass page

        }*/

        await notebookPasspage.RequestPage(pageNum,0.3f);

        OnWritingShakeNotebook?.Invoke(this, 0.5f);
        OnWritingNotebookSound?.Invoke(this, null);
        fade.StartEffect();
    }

    void OnWritingFinished()
    {
        fade.OnComplete -= OnWritingFinished;
        OnWritingWordFinished?.Invoke(this, wordReference);
        processManager.UnregisterProcess();
        isInWritingFade = false;
    }

    public void EraseAnim()
    {
        processManager.RegisterProcess();
        fade.OnEraseProgress += eraseParticles;
        fade.OnComplete += OnEraseFinished;
        fade.StartEffect(false);
    }
    public void eraseParticles(float progress)
    {
        EraseParticles.GetComponent<ParticleSystem>().Play();
        EraseParticles.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(text.preferredWidth,0,0), progress);
    }
    void OnEraseFinished()
    {
        fade.OnComplete -= OnEraseFinished;
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        fade.OnEraseProgress -= eraseParticles;

        processManager.UnregisterProcess();
    }

    public void RefreshWord(Component sender, object obj)
    {
        if (wordReference == null) return;

        if (wordReference.GetInactiveStateSeen() && !wordReference.GetEraseState())
        {
            if (isCross) return;

            bool needPassPage = pageNum != actualPage ? true : false;

            btn.enabled = false;
            strikethrough.SetActive(true);

            CrossOutWord(needPassPage);
        }
    }
    public void ReplaceWord(WordData word)
    {
        text.text = wordReference.GetName();

        ReplaceWordAnim( text, false, text, true, word.GetName());
        
        OnInactiveReplaceWord?.Invoke(this, wordReference);
        wordReference = word;
        word.SetIsFound();
        btn.enabled = true;

        foreach (WordData ascendat in word.SearchForWordsThatReplaceRetroactive())
        {
            // si alguna de las palabras anterior fue puesta en el board, activa el placed automático al remplasarce
            if(ascendat.GetPlacedInBoard()) PendingToAddBoard = true;
        }
    }

    public async Task ReplaceWordAnim(TMP_Text first, bool isTransparent1, TMP_Text second, bool isTransparent2, string txt = "")
    {
        processManager.RegisterProcess();
        fade.SetBlank(1);

        await notebookPasspage.RequestPage(pageNum, 1f);

        if (isCross) EraseCrossWord();
        OnWritingShakeNotebook?.Invoke(this, 0.5f);
        fade.StartEffect(isTransparent1);
        if (!isTransparent1) fade.OnEraseProgress += eraseParticles;
        await Task.Delay(500);
        EraseParticles.GetComponent<ParticleSystem>().Stop();
        if (first == second) first.text = txt;
        fade.gameObject.GetComponent<FadeWordsEffect>().StartEffect(isTransparent2);
        await Task.Delay(500);
        processManager.UnregisterProcess();
    }

    public void ReplaceWordInstantly(WordData word)
    {
        wordReference = word;
        word.SetIsFound();
        text.text = wordReference.GetName();
    }

    public void SetIsFinalReport()
    {
        isInFinalreport = true;
    }

        Vector3 CrossOriginalPos;
    public async Task CrossOutWord(bool needPassPage )
    {
        processManager.RegisterProcess();
        OnInactiveReplaceWord?.Invoke(this, wordReference);

        await notebookPasspage.RequestPage(pageNum, 0.3f);

        RectTransform line = strikethrough.GetComponent<RectTransform>();
        CrossOriginalPos = line.localPosition;
        line.DOSizeDelta(new Vector2(text.GetComponent<RectTransform>().sizeDelta.x, line.sizeDelta.y), 0.3f).OnComplete(() => processManager.UnregisterProcess());
        OnCrossWordSound?.Invoke(this, null);
        isCross = true;
    }


    public void EraseCrossWord()
    {
        processManager.RegisterProcess();
        RectTransform line = strikethrough.GetComponent<RectTransform>();
        
        line.pivot = new Vector2(1, 0);
        Vector2 _originalPosition = new Vector2(line.sizeDelta.x, line.localPosition.y);
        line.localPosition = new Vector2(line.sizeDelta.x, line.localPosition.y);
        line.DOSizeDelta(new Vector2(0, line.sizeDelta.y), 0.3f).OnComplete(() =>
        {
            line.pivot = new Vector2(0, 0.5f);
            line.localPosition = new Vector2(CrossOriginalPos.x, CrossOriginalPos.y);
            isCross = false;
            btn.enabled = true;
            processManager.UnregisterProcess();
        });
    }

    bool takeDossierWithWordOnBeginningOnce;
    bool isInFinalreport;
    public void SetSelectedWord()
    {
        if(isInFinalreport)
        {
            OnSendReportAutomatly?.Invoke(this, null);
            isInFinalreport = false;
            return;
        }

       text.text = "<u>" + wordReference.GetName() + "</u>";
        if (actualView == ViewStates.BoardView) text.text = wordReference.GetName();
        WordSelectedInNotebook.Notebook.SetSelectedWord(wordReference);
        isActiveInBoard = false;
        btn.enabled = false;
        if (actualView == ViewStates.PCView)
        {
            Invoke("UnSelectWord", 0.3f);
        }
        if (actualView == ViewStates.OnTakeSomeInBoard) OnButtonElement?.Invoke(this, ViewStates.BoardView);
        
        
        //

        if (actualView == ViewStates.DossierView && takeDossierWithWordOnBeginningOnce) OnButtonElement?.Invoke(this, ViewStates.DossierView);

        takeDossierWithWordOnBeginningOnce = false;
    }
    void UnSelectWord()
    {
        text.text = wordReference.GetName();
        btn.enabled = true;
    }

    public void ClearUnderline()
    {
        
        if (isActiveInBoard) return;
        if (isinactive) return;
        if (actualView != ViewStates.OnTakeSomeInBoard && actualView != ViewStates.BoardView) if (!wordReference.GetInactiveStateSeen()) btn.enabled = true;
        if (isInWritingFade) return;
        text.text = wordReference.GetName();
        
    }


    public WordData GetWord() { return wordReference; }


   public void TryActiveWord(bool x)
    {
        if (!wordReference.GetInactiveStateSeen()) btn.enabled = x;

        if (actualView != ViewStates.BoardView && actualView != ViewStates.OnTakeSomeInBoard) return;

        if (wordReference.GetInactiveStateSeen())
        {
            btn.enabled = x;
        }

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

    public void ApplyThicknessDirectly(float value)
    {
        isActiveInBoard = false;
        Material mat = GetMat();
        mat.SetFloat(ShaderUtilities.ID_FaceDilate, value);
    }

    public void ApplyThicknessAnim(bool x)
    {
        if (PendingToAddBoard) return;
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
    public void InactiveDirectly()
    {
        thicknessSequence?.Kill();

        text.text = wordReference.GetName();

        btn.enabled = false;
    }
    public void ThicknessOn(float targetValue = 0.35f, float duration = 0.3f)
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
        if (materialName == null) return text.GetComponent<ShaderMaterialManager>().GetFirstMat();
        return text.GetComponent<ShaderMaterialManager>().GetHighLigthMaterial(materialName.Replace("\"", ""));
    }

    ViewStates actualView = ViewStates.DossierView;
    public void CheckView(Component sender,object obj)
    {
        actualView = (ViewStates)obj;

    }


    public void OnBoardUpdate(Component sender,object obj)
    {
        if (PendingToAddBoard)
        {
            btn.GetComponent<OnClickDownEvent>().onPointerDown.Invoke();
            PendingToAddBoard = false;
        }
    }

    public void SetActualPage(Component sender, object obj)
    {
        actualPage = (int)obj;
    }

    public void SetInactive(bool x) => isinactive = x;
}

