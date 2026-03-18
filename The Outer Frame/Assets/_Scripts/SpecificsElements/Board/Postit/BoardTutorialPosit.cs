using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardTutorialPosit : MonoBehaviour
{
    Vector3 GeneralViewPos;
    [SerializeField] Transform BoardViewPos;
    [SerializeField] Transform OutOfView;
    [SerializeField] Transform OutOfViewfromBoard;
    [SerializeField] int amountOfWordsToShowPosit;
    [SerializeField] GameEvent OnReturningPositBlock;
    int amountOfWordsTaked;
    bool pendingToShowPosit;
    bool thereIsAnIdeaPendingToPut;
    GameObject Posit;
    Sequence moveSequence;
    bool wordWasPlacedOnBoard;
    bool isInTutorial;
    bool inactive;
    ViewStates LastView;
    ViewStates ActualView;
    bool NotShowedYet = true;

    private void Start()
    {
        Posit = transform.GetChild(0).gameObject;
        GeneralViewPos = Posit.transform.position;
        Posit.SetActive(false);
    }

    public void CheckWordsTaked(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        amountOfWordsTaked++;

        if(amountOfWordsTaked == amountOfWordsToShowPosit) pendingToShowPosit = true;
    }


    public void CheckViewWithDelay(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        ActualView = (ViewStates)obj;
        LastView = ActualView;

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        if (ActualView == ViewStates.BoardView || ActualView == ViewStates.GeneralView || ActualView == ViewStates.OnTakenPaperView || ActualView == ViewStates.OnTakeSomeInBoard || ActualView == ViewStates.DossierView || ActualView == ViewStates.PauseView)
        {
            return;
        }

        ActiveDesactivePosIt();
    }

    void ActiveDesactivePosIt()
    {
        if (inactive) return;
        if (wordWasPlacedOnBoard)
        {
            Posit.SetActive(false);
            inactive = true;
            return;
        }

        Posit.SetActive(true);
        NotShowedYet = false;
    }

    public void CheckView(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        ActualView = (ViewStates)obj;
        if (LastView == null) LastView = ActualView;

        if (ActualView == ViewStates.BoardView)
        {
            movePosit(BoardViewPos.position);
        }
        else if (ActualView == ViewStates.GeneralView)
        {
            movePosit(GeneralViewPos);
        }
    }


    public void GetTrueConditionalIdeas(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        thereIsAnIdeaPendingToPut = true;
        Debug.Log("At least one idea is pendig to show");
    }

    void movePosit(Vector3 MoveTo)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        moveSequence = DOTween.Sequence();

        moveSequence.Append(Posit.transform.DOMove(MoveTo,0.5f)).SetEase(Ease.InOutCirc);
    }

    public void OnPlacedWordOnBoard(Component sender, object obj)
    {
        if (NotShowedYet) inactive = true;
        if (inactive) return;
        if (isInTutorial) return;
        if (LastView != ViewStates.OnTakeSomeInBoard) return;
        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        wordWasPlacedOnBoard = true;

        MoveOutView(OutOfView);
    }

    public void CheckPendingBordsToPlaceOnBoard(Component sender, object obj)
    {
        if (inactive) return;
        bool arePendingWords = (bool)obj;
        if (arePendingWords) return;
        if (LastView == ViewStates.OnTakeSomeInBoard) return;

        wordWasPlacedOnBoard = true;

        MoveOutView(OutOfViewfromBoard);
    }

    void MoveOutView(Transform toMove)
    {

        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        moveSequence = DOTween.Sequence();

        OnReturningPositBlock?.Invoke(this, null);

        Destroy(Posit.GetComponent<ButtonElement>());

        moveSequence.Append(Posit.transform.DOMove(toMove.transform.position, 0.5f)).SetEase(Ease.InOutCirc).
            Join(Posit.transform.DORotate(toMove.transform.rotation.eulerAngles, 0.5f)).
            OnComplete(() =>
            Destroy(gameObject)
            );

    }

    public void SetInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;
    }
}
