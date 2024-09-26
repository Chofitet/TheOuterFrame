using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WalkmanMoveController : MonoBehaviour
{
    [SerializeField] Transform InitPos;
    [SerializeField] Transform FinalPos;
    Sequence WalkmanSequence;
    public void InPause(Component sender, object obj)
    {
        if (WalkmanSequence != null && WalkmanSequence.IsActive()) WalkmanSequence.Kill();

        WalkmanSequence = DOTween.Sequence();

        WalkmanSequence.PrependInterval(0.6f)
                         .Append(transform.DOMove(InitPos.position, 0.3f)).SetEase(Ease.InOutQuad)
                         .Join(transform.DORotate(InitPos.rotation.eulerAngles,0.3f)).SetEase(Ease.InOutQuad);
    }

    public void OutPause(Component sender, object obj)
    {
        if (WalkmanSequence != null && WalkmanSequence.IsActive()) WalkmanSequence.Kill();

        WalkmanSequence = DOTween.Sequence();

        WalkmanSequence.Append(transform.DOMove(FinalPos.position, 0.3f)).SetEase(Ease.InOutQuad)
                       .Join(transform.DORotate(FinalPos.rotation.eulerAngles, 0.3f)).SetEase(Ease.InOutQuad);
    }
}
