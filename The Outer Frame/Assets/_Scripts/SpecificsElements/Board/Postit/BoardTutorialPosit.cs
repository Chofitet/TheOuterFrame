using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BoardTutorialPosit : MonoBehaviour
{

    [Header("Position Placed in board")]
    [SerializeField] Transform GeneralViewPos;
    [SerializeField] Transform BoardViewPos;
    [SerializeField] Transform OutOfViewfromBoard;
    [Header("Position Taked next to notebook")]
    [SerializeField] Transform NextToNotebookPos;
    [SerializeField] Transform OutOfView;
    [SerializeField] int amountOfWordsToShowPosit;
    [SerializeField] List<ExtraPositsData> posits = new List<ExtraPositsData>();
    [SerializeField] GameObject ExtraPosits;


    [SerializeField] GameEvent OnButtonElement;
    int amountOfWordsTaked;
    bool pendingToShowPosit;
    bool thereIsAnIdeaPendingToPut;
    GameObject Posit;
    Sequence moveSequence;
    int wordWasPlacedOnBoard;
    bool isInTutorial;
    bool inactive;
    ViewStates LastView;
    ViewStates ActualView;
    int CamChangesIndex = 0; 


    private void Start()
    {
        Posit = transform.GetChild(0).gameObject;
        AppearPosits(false,true);
    }

    public void CheckWordsTaked(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        amountOfWordsTaked++;

        bool viewCheck = ActualView == ViewStates.PCView || ActualView == ViewStates.TVView || ActualView == ViewStates.ProgressorView || ActualView == ViewStates.PinchofonoView;

        if (amountOfWordsTaked == amountOfWordsToShowPosit && viewCheck) pendingToShowPosit = true;

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;
        if (viewCheck) ActiveDesactivePosIt(true);
    }

    bool isChangingCam;
    public void CheckView(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        ActualView = (ViewStates)obj;
        if (LastView == null) LastView = ActualView;
        
        if(!isChangingCam) isChangingCam = true;
        else isChangingCam = false;

        if (ActualView == ViewStates.BoardView)
        {
            MoveExtraPosit(BoardViewPos);
            movePosit(BoardViewPos);
        }
        else if (ActualView == ViewStates.GeneralView)
        {
            movePosit(GeneralViewPos);
        }
    }

    public void MoveToGeneralViewPos(Component sender, object obj)
    {
        movePosit(GeneralViewPos);
    }

    Coroutine CheckWhitDelayCoroutine;

    public void CheckViewWithDelay(Component sender, object obj)
    {
        if (CheckWhitDelayCoroutine != null) StopCoroutine(CheckWhitDelayCoroutine);
        
        CheckWhitDelayCoroutine = StartCoroutine(CheckViewWithDelay((ViewStates)obj));
    }

    IEnumerator CheckViewWithDelay(ViewStates view)
    {
        yield return new WaitForSeconds(0.5f);

        if (inactive) yield break;
        if (isInTutorial) yield break;
        ActualView = view;

        LastView = ActualView;


        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) yield break;

        if (ActualView == ViewStates.BoardView || ActualView == ViewStates.GeneralView || ActualView == ViewStates.OnTakenPaperView || ActualView == ViewStates.OnTakeSomeInBoard || ActualView == ViewStates.DossierView || ActualView == ViewStates.PauseView || ActualView == ViewStates.BoardZoomView)
        {
            isChangingCam = false;
            yield break;
        }

        if (isChangingCam) ActiveDesactivePosIt();
        isChangingCam = false;

    }

    public void CkeckOnApprove(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        ActiveDesactivePosIt();
    }

    void ActiveDesactivePosIt(bool NoCountCamChanges2 = false)
    {
        if (inactive) return;
        if (wordWasPlacedOnBoard >= amountOfWordsToShowPosit)
        {
            AppearPosits(false);
            inactive = true;
            return;
        }

        AppearPosits(true,false, NoCountCamChanges2);
    }
    public void GetTrueConditionalIdeas(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        thereIsAnIdeaPendingToPut = true;
        Debug.Log("At least one idea is pendig to show");

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;
        if (ActualView == ViewStates.PCView || ActualView == ViewStates.TVView || ActualView == ViewStates.ProgressorView || ActualView == ViewStates.PinchofonoView) ActiveDesactivePosIt();
    }
    bool isTaken;
    public void MoveToTakedPosition(Component sender, object obj)
    {
        if (LastView == ViewStates.OnTakeSomeInBoard) OnButtonElement?.Invoke(this, ViewStates.BoardView);

        ReparentExtraPosits(transform);
        movePosit(NextToNotebookPos);
        isTaken = true;

    }

    Sequence MoveExtraPositsSequence;
    public void BackToBoardPosition(Component sender, object obj)
    {
        Transform toMove = obj as Transform;

        if (toMove == null) toMove = GeneralViewPos;
        if(ActualView != ViewStates.OnTakeSomeInBoard)MoveExtraPosit(GeneralViewPos);
        movePosit(toMove);
        isTaken = false;

    }

    void movePosit(Transform MoveTo)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        moveSequence = DOTween.Sequence();

        moveSequence.Append(Posit.transform.DOMove(MoveTo.position, 0.5f)).SetEase(Ease.InOutCirc)
            .Join(Posit.transform.DORotate(MoveTo.rotation.eulerAngles, 0.5f)).OnComplete(OnCompletePositMove);
    }

    void OnCompletePositMove()
    {
        if (!isTaken)
        {
            ReparentExtraPosits(Posit.transform);
        }
    }

    void MoveExtraPosit(Transform ToMove)
    {
        if (MoveExtraPositsSequence != null && MoveExtraPositsSequence.IsActive()) MoveExtraPositsSequence.Kill();

        MoveExtraPositsSequence = DOTween.Sequence();

        MoveExtraPositsSequence.Append(ExtraPosits.transform.DOMove(ToMove.position, 0.5f).SetEase(Ease.InOutCirc));

    }


    public void CheckPendingBordsToPlaceOnBoard(Component sender, object obj)
    {
        if (inactive) return;
        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;
        bool arePendingWords = (bool)obj;
        if (arePendingWords)
        {
            return;
        }


        wordWasPlacedOnBoard++;

        if (!isTaken) MoveOutView(OutOfViewfromBoard);
        else
        {
            MoveExtraPosit(OutOfViewfromBoard);
            MoveOutView(OutOfView);
        }
    }

    public void PlaceAPhotBeforeActivatePosit(Component sender, object obj)
    {
        if (ActualView != ViewStates.BoardView && ActualView != ViewStates.OnTakeSomeInBoard) return;
        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit)
        {
            if (wordWasPlacedOnBoard <= amountOfWordsToShowPosit) return;
            inactive = true;
        }
    }

    void MoveOutView(Transform toMove)
    {

        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        moveSequence = DOTween.Sequence();

        Destroy(Posit.GetComponent<ButtonElement>());

        moveSequence.Append(Posit.transform.DOMove(toMove.transform.position, 0.5f)).SetEase(Ease.InOutCirc).
            Join(Posit.transform.DORotate(toMove.transform.rotation.eulerAngles, 0.5f)).
            OnComplete(() =>
            Destroy(gameObject)
            );

    }

    int lastPositAppeared;
    void AppearPosits(bool boolState, bool NoCountCamChanges = false, bool NoCountCamChanges2 = false)
    {
        if (!NoCountCamChanges)
        {
            if(!NoCountCamChanges2 || CamChangesIndex < 1) CamChangesIndex++;
            if (posits.Count - 1 < lastPositAppeared) return;
            if (posits[lastPositAppeared].AmountOfCamChanges != CamChangesIndex) return;
        }

        if (boolState)
        {
            if (lastPositAppeared >= posits.Count) return;
            foreach (ExtraPositsData ExtraPosit in posits)
            {
                if (posits.IndexOf(ExtraPosit) == -1) break;

            }
            posits[lastPositAppeared].posit.SetActive(true);
            lastPositAppeared++;
        }
        else
        {
            foreach (ExtraPositsData ExtraPosit in posits)
            {
                ExtraPosit.posit.SetActive(false);
            }
        }
    }

    void ReparentExtraPosits(Transform parent)
    {
        ExtraPosits.transform.SetParent(parent, true);
    }

    public void SetInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;
    }


}
[Serializable]
public class ExtraPositsData
{
    public GameObject posit;
    public int AmountOfCamChanges;

}