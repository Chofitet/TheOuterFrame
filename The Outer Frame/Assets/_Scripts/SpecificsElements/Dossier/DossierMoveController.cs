using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DossierMoveController : MonoBehaviour
{
    [SerializeField] Transform TakenPosition;
    [SerializeField] Transform LeavePosition;
    [SerializeField] Transform InitShowInBoardPosition;
    [SerializeField] Transform FinalShowInBoardPosition;
    [SerializeField] GameEvent OnBackToGeneralView;
    [SerializeField] GameEvent OnChangeView;
    [SerializeField] GameEvent OnActionPlanDossier;
    [SerializeField] GameEvent OnWriteDossier;
    [SerializeField] GameEvent OnSetTimeSpeed;
    [SerializeField] GameEvent OnNotebookTake;
    Animator DossierAnim;
    Sequence AddIdeaSequence;
    Sequence MoveDossierSequence;
    bool isAddingIdea;

    private bool isFollowingTarget;
    private float lerpTime;
    [SerializeField] private float moveDuration = 0.7f;


    void Update()
    {
        if (isFollowingTarget && TakenPosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, TakenPosition.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, TakenPosition.rotation, lerpTime);
        }
    }

    public void ShowInBoard(Component sender, object obj)
    {
        if (AddIdeaSequence != null && AddIdeaSequence.IsActive()) AddIdeaSequence.Kill();
        DossierAnim = transform.GetChild(0).GetComponent<Animator>();
        AddIdeaSequence = DOTween.Sequence();
        GetComponent<Animator>().enabled = false;
        transform.position = InitShowInBoardPosition.position;
        transform.rotation = InitShowInBoardPosition.rotation;
        OnActionPlanDossier?.Invoke(this, null);
        isAddingIdea = true;
        // Reseteamos el tiempo de Lerp y activamos el seguimiento del target
        lerpTime = 0;
        isFollowingTarget = false; // Inicialmente no sigue al target
        DossierAnim.SetTrigger("instantActionplan");
        
        AddIdeaSequence.AppendInterval(0.6f)
            .Append(transform.DOMove(FinalShowInBoardPosition.position, 0.8f).SetEase(Ease.OutSine))
            .AppendCallback(() =>
            {
                OnWriteDossier?.Invoke(this, 1.5f);
            })
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                OnChangeView?.Invoke(this, ViewStates.DossierView);
                OnSetTimeSpeed?.Invoke(this, 1);
            })
            .AppendCallback(() =>
            {
            isFollowingTarget = true;
            })
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, moveDuration)
                .SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
                isFollowingTarget = false; // Detenemos el seguimiento al finalizar el movimiento
                isAddingIdea = false;
                AddIdeaSequence.Kill();
                DossierAnim.ResetTrigger("open");
            });
    }

    public void CancelAddIdeaAnim(Component sender, object obj)
    {
        if((ViewStates)obj == ViewStates.GeneralView)
        {
            if (AddIdeaSequence != null && AddIdeaSequence.IsActive()) AddIdeaSequence.Kill();
            else return;
            AddIdeaSequence = DOTween.Sequence();

            AddIdeaSequence.AppendCallback(() =>
            {
                OnChangeView?.Invoke(this, ViewStates.DossierView);
                OnSetTimeSpeed?.Invoke(this, 1);
                OnNotebookTake?.Invoke(this, null);
            })
            .AppendCallback(() =>
            {
                isFollowingTarget = true;
            })
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, moveDuration)
             .SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
                
                isFollowingTarget = false; 
                                           
                isAddingIdea = false;
                DossierAnim.ResetTrigger("open");
            });
        }
    }

    public void TakeDossier(Component sender, object obj)
    {
        if (isAddingIdea) return;
        if (MoveDossierSequence != null && MoveDossierSequence.IsActive()) MoveDossierSequence.Kill();

        MoveDossierSequence = DOTween.Sequence();

        MoveDossierSequence.Append(transform.DOMove(TakenPosition.position, 0.5f)).SetEase(Ease.InOutSine)
                            .Join(transform.DORotate(TakenPosition.rotation.eulerAngles, 0.3f)).SetEase(Ease.InExpo);
    }
    public void LeaveDossier(Component sender, object obj)
    {
        if (isAddingIdea) return;
        if (MoveDossierSequence != null && MoveDossierSequence.IsActive()) MoveDossierSequence.Kill();

        MoveDossierSequence = DOTween.Sequence();

        MoveDossierSequence.Append(transform.DOMove(LeavePosition.position, 0.5f)).SetEase(Ease.InOutSine)
                            .Join(transform.DORotate(LeavePosition.rotation.eulerAngles, 0.3f)).SetEase(Ease.InExpo);
    }
}