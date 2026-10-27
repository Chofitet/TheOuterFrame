using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField] List<GameObject> posits = new List<GameObject>(); 

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
    bool NotShowedYet = true;

    private void Start()
    {
        Posit = transform.GetChild(0).gameObject;
        AppearPosits(false);
    }

    public void CheckWordsTaked(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        amountOfWordsTaked++;

        if(amountOfWordsTaked == amountOfWordsToShowPosit) pendingToShowPosit = true;

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;
        if (ActualView == ViewStates.PCView || ActualView == ViewStates.TVView || ActualView == ViewStates.ProgressorView || ActualView == ViewStates.PinchofonoView) ActiveDesactivePosIt();
    }

    public void CheckView(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        ActualView = (ViewStates)obj;
        if (LastView == null) LastView = ActualView;

        if (ActualView == ViewStates.BoardView)
        {
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

    public void CheckViewWithDelay(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        ActualView = (ViewStates)obj;
        
        LastView = ActualView;
        

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        if (ActualView == ViewStates.BoardView || ActualView == ViewStates.GeneralView || ActualView == ViewStates.OnTakenPaperView || ActualView == ViewStates.OnTakeSomeInBoard || ActualView == ViewStates.DossierView || ActualView == ViewStates.PauseView || ActualView == ViewStates.BoardZoomView)
        {
            return;
        }

        ActiveDesactivePosIt();
    }

    public void CkeckOnApprove(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;

        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        ActiveDesactivePosIt();
    }

    void ActiveDesactivePosIt()
    {
        if (inactive) return;
        if (wordWasPlacedOnBoard >= amountOfWordsToShowPosit)
        {
            AppearPosits(false);
            inactive = true;
            return;
        }

        AppearPosits(true);
        NotShowedYet = false;
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
        if(LastView == ViewStates.OnTakeSomeInBoard) OnButtonElement?.Invoke(this, ViewStates.BoardView);
        movePosit(NextToNotebookPos);
        isTaken = true;
    }

    public void BackToBoardPosition(Component sender, object obj)
    {
        Transform toMove = obj as Transform;

        if (toMove == null) toMove = GeneralViewPos;

        movePosit(toMove);
        isTaken = false;
    }

    void movePosit(Transform MoveTo)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        moveSequence = DOTween.Sequence();

        moveSequence.Append(Posit.transform.DOMove(MoveTo.position, 0.5f)).SetEase(Ease.InOutCirc)
            .Join(Posit.transform.DORotate(MoveTo.rotation.eulerAngles, 0.5f));
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
        else MoveOutView(OutOfView);
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
    void AppearPosits(bool boolState)
    {
        if(boolState)
        {
            if (lastPositAppeared >= posits.Count) return;
            foreach (GameObject posit in posits)
            {
                if (posits.IndexOf(posit) == -1) break;
                if(posits.IndexOf(posit) < lastPositAppeared)
                {
                 //   posit.GetComponent<BoxCollider>().enabled = false;
                }
                
            }
            posits[lastPositAppeared].SetActive(true);
            posits[lastPositAppeared].GetComponent<BoxCollider>().enabled = true;
            lastPositAppeared++;
        }
        else
        {
            foreach(GameObject posit in posits)
            {
                posit.GetComponent<BoxCollider>().enabled = true;
                posit.SetActive(false);
            }
        }
    }

    public void SetInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;
    }
}
