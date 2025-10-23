using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InactiveIdeaAnimation : MonoBehaviour
{
    Sequence InactiveAnimSequence;
    [SerializeField] Transform PivotPoint;
    [SerializeField] Image LampOff;
    [SerializeField] Image LampOn;
    [SerializeField] GameObject TakeOffBoardBTN;
    [SerializeField] Transform TakeOutPosition;
    [SerializeField] GameEvent OnButtonElement;
    [SerializeField] GameEvent OnDiscartIdea;
    bool once;



    public void InactiveAnim()
    {
        if (once) return;
        if(InactiveAnimSequence != null && InactiveAnimSequence.IsActive()) InactiveAnimSequence.Kill();

        InactiveAnimSequence = DOTween.Sequence();

        once = true;
        float angle = 0;
        Vector3 pivotDescentrado = PivotPoint.position;
        TakeOffBoardBTN.SetActive(true);

        InactiveAnimSequence.Append(DOTween.To(() => angle, x =>
        {
            float delta = x - angle;
            angle = x;
            transform.RotateAround(pivotDescentrado, Vector3.forward, delta);

        }, 10f, 2f).SetEase(Ease.OutElastic))
        .Join(LampOff.DOFade(1, 1.2f))
        .Join(LampOn.DOFade(0, 1.2f));

    }

    public void TakeOutIdea()
    {
        GetComponent<MoveBoardElementsToPos>().MoveToTakeOutPos(this, TakeOutPosition);
        OnDiscartIdea?.Invoke(this, null);
        Invoke("BackToBoardView", 0.2f);
    }

    void BackToBoardView()
    {
        OnButtonElement?.Invoke(this, ViewStates.BoardView);
    }

    [ContextMenu("InactiveTest")]
    public void test()
    {
        InactiveAnim();
    }
}
