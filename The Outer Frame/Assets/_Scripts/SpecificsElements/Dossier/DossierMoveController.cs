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
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] GameEvent OnBackPositToBoardPos;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform InitCameraPivot;
    Animator DossierAnim;
    Sequence AddIdeaSequence;
    Sequence MoveDossierSequence;
    bool isAddingIdea;
    bool isUp;

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

        OnDisableInput?.Invoke(this, null);

        AddIdeaSequence
            .Append(transform.DOMove(FinalShowInBoardPosition.position, 0.8f).SetEase(Ease.OutSine))
            .AppendInterval(0.4f)
            .AppendCallback(() =>
            {
                OnWriteDossier?.Invoke(this, 1.5f);
            })
            .AppendInterval(0.5f)
            .AppendCallback(()=>
            {
                OnBackPositToBoardPos?.Invoke(this, posit);
            })
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                OnChangeView?.Invoke(this, ViewStates.DossierView);
                TimeManager.timeManager.NormalizeTime();
                transform.SetParent(cameraPivot);
                
            })
            .AppendCallback(() =>
            {
            isFollowingTarget = true;
            })
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, 0.5f)
                .SetEase(Ease.InQuart))
            .OnComplete(() =>
            {
                // Detenemos el seguimiento al finalizar el movimiento
                transform.SetParent(InitCameraPivot);
                isAddingIdea = false;
                AddIdeaSequence.Kill();
                DossierAnim.ResetTrigger("open");
                OnEnableInput?.Invoke(this, null);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() => isFollowingTarget = false);
    }
    ViewStates actualView;
    public void CancelAddIdeaAnim(Component sender, object obj)
    {

        if (actualView == ViewStates.GeneralView)
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
             .SetEase(Ease.InSine))
            .OnComplete(() =>
            {
                
                isFollowingTarget = false; 
                                           
                isAddingIdea = false;
                DossierAnim.ResetTrigger("open");
            });
        }
    }

    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
    }

    bool isReturningFromProgressor;

    public void SetIsReturningFromProgressor(Component sender, object obj)
    {
        isReturningFromProgressor = true;
    }

    public void SetFalseIsReturningFromProgressor(Component sender, object obj)
    {
        isReturningFromProgressor = false;
    }

    public void TakeDossier(Component sender, object obj)
    {
        if (isReturningFromProgressor)
        {
            TakeDossierFromProgressor();
            isReturningFromProgressor = false;
            return;
        }
        if (isAddingIdea) return;
        if (MoveDossierSequence != null && MoveDossierSequence.IsActive()) MoveDossierSequence.Kill();
        isUp = true;
        MoveDossierSequence = DOTween.Sequence();

        MoveDossierSequence.Append(transform.DOMove(TakenPosition.position, 0.4f)).SetEase(Ease.InOutSine)
                            .Join(transform.DORotate(TakenPosition.rotation.eulerAngles, 0.3f)).SetEase(Ease.InOutSine);
    }

    void TakeDossierFromProgressor()
    {
        float takeVelocity = 0.4f;

        if (MoveDossierSequence != null && MoveDossierSequence.IsActive()) MoveDossierSequence.Kill();

        MoveDossierSequence = DOTween.Sequence();

        // Iniciar la secuencia
        MoveDossierSequence
            .AppendCallback(() =>
            {
            // Activar el seguimiento en el Update
            isFollowingTarget = true;
            })
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, takeVelocity, takeVelocity)
                .SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
            // Desactivar el seguimiento y resetear estados
            isFollowingTarget = false;
                isReturningFromProgressor = false;
            });
    }
    public void LeaveDossier(Component sender, object obj)
    {
        float LeaveVelocity = 0.5f;
        if (isUp && actualView != ViewStates.GeneralView && actualView != ViewStates.OnTakenPaperView && actualView != ViewStates.GameOverView) LeaveVelocity = 0.3f;
        if (isAddingIdea) return;
        if (MoveDossierSequence != null && MoveDossierSequence.IsActive()) MoveDossierSequence.Kill();

        MoveDossierSequence = DOTween.Sequence();
        isUp = false;
        MoveDossierSequence
            .Append(transform.DOMove(LeavePosition.position, LeaveVelocity)).SetEase(Ease.InOutSine)
            .Join(transform.DORotate(LeavePosition.rotation.eulerAngles, LeaveVelocity - 0.2f)).SetEase(Ease.InOutSine)
        .OnComplete(() =>
         {
             transform.GetChild(0).transform.DOLocalRotate(Vector3.zero, 0.2f);
         });
    }

    public void CompleteSlotsProgressorAnim(Component sender, object obj)
    {

        if (MoveDossierSequence != null && MoveDossierSequence.IsActive()) MoveDossierSequence.Kill();
        MoveDossierSequence = DOTween.Sequence();

        MoveDossierSequence.Append(transform.DORotate(new Vector3(1, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 0.2f))
            .AppendInterval(0.4f)
            .Append(transform.DORotate(new Vector3(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 0.2f));

    }

    GameObject posit;

    public void LastTakedPosit(Component sender, object obj)
    {
        posit = (GameObject)obj;
    }
}
