using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardTutorialPosit : MonoBehaviour
{
    Vector3 GeneralViewPos;
    [SerializeField] Transform BoardViewPos;
    [SerializeField] int amountOfWordsToShowPosit;
    int amountOfWordsTaked;
    bool pendingToShowPosit;
    bool thereIsAnIdeaPendingToPut;
    GameObject Posit;
    Sequence moveSequence;
    bool wordWasPlacedOnBoard;
    bool isInTutorial;
    bool inactive;

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
        ViewStates actualView = (ViewStates)obj;


        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        if (actualView == ViewStates.BoardView || actualView == ViewStates.GeneralView || actualView == ViewStates.OnTakenPaperView || actualView == ViewStates.OnTakeSomeInBoard || actualView == ViewStates.DossierView || actualView == ViewStates.PauseView)
        {
            return;
        }

        ActiveDesactivePosIt();
    }

    public void OnSendingActionPlan(Component sender, object obj)
    {
        if (inactive) return;
        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

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
    }

    public void CheckView(Component sender, object obj)
    {
        if (inactive) return;
        if (isInTutorial) return;
        ViewStates actualView = (ViewStates)obj;

        if (actualView == ViewStates.BoardView)
        {
            movePosit(BoardViewPos.position);
        }
        else if (actualView == ViewStates.GeneralView)
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
        if (inactive) return;
        if (isInTutorial) return;
        if (!thereIsAnIdeaPendingToPut || !pendingToShowPosit) return;

        wordWasPlacedOnBoard = true;
        
    }

    public void SetInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;
    }
}
