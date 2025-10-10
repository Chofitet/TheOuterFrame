using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyMoveController : MonoBehaviour
{
    [SerializeField] Transform OutPrintPos;
    [SerializeField] AnimationCurve ToFinalDeskPosCurve;
    [SerializeField] Transform DeskPlaceFinalPos;
    [SerializeField] GameEvent OnCandyEating;
    [SerializeField] GameEvent DisableInput;
    [SerializeField] GameEvent EnableInput;
    [SerializeField] float TimeOutPrint;
    [SerializeField] float TimeWiting;
    [SerializeField] float TimePlacing;
    Sequence MoveCady;

    public void TakeCandy(Component sender,object obj)
    {
        GameObject candy = (GameObject)obj;
        if (MoveCady != null && MoveCady.IsActive()) MoveCady.Kill();

        MoveCady = DOTween.Sequence();
        candy.transform.SetParent(OutPrintPos);
        DisableInput?.Invoke(this, null);

        MoveCady.Append(candy.transform.DOMove(OutPrintPos.position, TimeOutPrint).SetEase(Ease.OutSine))
         .Join(candy.transform.DORotate(OutPrintPos.rotation.eulerAngles, TimeOutPrint)).SetEase(Ease.InQuart)
         .AppendInterval(TimeWiting)
         .Append(candy.transform.DOMoveX(DeskPlaceFinalPos.position.x, TimePlacing).SetEase(ToFinalDeskPosCurve))
         .Join(candy.transform.DOMoveY(DeskPlaceFinalPos.position.y, TimePlacing).SetEase(ToFinalDeskPosCurve))
         .Join(candy.transform.DOMoveZ(DeskPlaceFinalPos.position.z, TimePlacing).SetEase(ToFinalDeskPosCurve))
         .Join(candy.transform.DORotate(DeskPlaceFinalPos.rotation.eulerAngles, 0.3f)).SetEase(Ease.Linear)
         .OnComplete(() =>
         {
             EnableInput?.Invoke(this, null);
         });

    }
}
