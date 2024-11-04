using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovePosItIdeaController : MonoBehaviour
{
    [SerializeField] Transform TakenBoardPosition;
    [SerializeField] Transform SideTakenIdeaPosition;
    [SerializeField] Transform OutOfBoard;
    [SerializeField] GameEvent OnSetIdeaTaken;
    bool isTaken;
    Sequence moveIdeaSequence;
    Vector3 InitPosition;
    Vector3 InitAngle;

    private void OnEnable()
    {
        InitPosition = transform.position;
        InitAngle = transform.rotation.eulerAngles;
    }

    public void TakeIdea(Component sender, object obj)
    {
        if (moveIdeaSequence != null && moveIdeaSequence.IsActive()) moveIdeaSequence.Kill();
        OnSetIdeaTaken?.Invoke(this, true);
        moveIdeaSequence = DOTween.Sequence();

        moveIdeaSequence.Append(transform.DOMove(TakenBoardPosition.position, 0.2f))
                .Join(transform.DORotate(TakenBoardPosition.rotation.eulerAngles, 0.2f));
    }

    public void LeaveIdea(Component sender, object obj)
    {
        if (moveIdeaSequence != null && moveIdeaSequence.IsActive()) moveIdeaSequence.Kill();

        moveIdeaSequence = DOTween.Sequence();

        moveIdeaSequence.Append(transform.DOMove(InitPosition, 0.2f))
                .Join(transform.DORotate(InitAngle, 0.2f))
                .OnComplete(() => { 
                    GetComponent<BoxCollider>().enabled = true;
                    OnSetIdeaTaken?.Invoke(this, false);
                });
    }

    public void MoveToSideIdea(Component sender, object obj)
    {
        if (moveIdeaSequence != null && moveIdeaSequence.IsActive()) moveIdeaSequence.Kill();

        moveIdeaSequence = DOTween.Sequence();

        moveIdeaSequence.Append(transform.DOMove(SideTakenIdeaPosition.position, 0.2f))
                .Join(transform.DORotate(SideTakenIdeaPosition.rotation.eulerAngles, 0.2f));
    }

    private void OnMouseUpAsButton()
    {
        GetComponent<BoxCollider>().enabled = false;
        TakeIdea(null, null);
    }

}
