using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] Transform OutPos;
    [SerializeField] Transform InPos;
    Sequence PaperMoveSequence;

    public void InPause(Component sender, object obj)
    {
        if (PaperMoveSequence != null && PaperMoveSequence.IsActive()) PaperMoveSequence.Kill();

        PaperMoveSequence = DOTween.Sequence();

        PaperMoveSequence.PrependInterval(0.6f)
                         .Append(transform.DOMove(InPos.position, 0.3f)).SetEase(Ease.InOutQuad);
    }

    public void OutPause(Component sender, object obj)
    {
        if (PaperMoveSequence != null && PaperMoveSequence.IsActive()) PaperMoveSequence.Kill();

        PaperMoveSequence = DOTween.Sequence();

        PaperMoveSequence.Append(transform.DOMove(OutPos.position, 0.3f)).SetEase(Ease.InOutQuad);
    }
}
