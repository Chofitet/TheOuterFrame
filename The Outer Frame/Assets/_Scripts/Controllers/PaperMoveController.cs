using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PaperMoveController : MonoBehaviour
{
    [SerializeField] Transform TakenPos;
    [SerializeField] GameObject ReportInstances;
    [SerializeField] Transform ReportPilePos;
    [SerializeField] Transform HoldRigthPos;
    [SerializeField] float takeDuration;
    [SerializeField] Transform PCSpot1;
    [SerializeField] Transform PCSpot2;
    [SerializeField] Transform DescartPos;
    [SerializeField] GameEvent OnPressButtomElement;
    [SerializeField] GameEvent OnSetPaperState;
    [SerializeField] GameEvent OnSetPaperTaken;
    private bool isMoving;
    GameObject currentPaper;
    bool isHolding;
    private Sequence moveSequence;
    private Sequence moveToPcSequence;
    private Sequence moveDescart;
    private Sequence swapPapersSequence;
    PaperState actualPaperState;
    List<GameObject> PapersQueue = new List<GameObject>();
    Vector3 TransformOffset;
    Vector3 RotationOffset;

    enum PaperState
    {
        Nothing,
        Taken,
        HoldingRight,
        first,
        Staked
    }

    private void Start()
    {
        actualPaperState = PaperState.first;
    }
    void SetPaperState(PaperState newstate)
    {
        switch (newstate)
        {
            case PaperState.Nothing:
                OnSetPaperState?.Invoke(null, false);
                break;
            case PaperState.Taken:
                OnSetPaperState?.Invoke(null, false);
                break;
            case PaperState.HoldingRight:
                OnSetPaperState?.Invoke(null, true);
                break;
        }
        actualPaperState = newstate;
        
    }

    //Declarar los distintos spots a los que puede ir

    //Una referencia que identifique qué papel está siendo agarrado

    //Un state machene que separe en que estado se está? hold,  

    //un instanciaSpot de cada tipo donde se holdeará los papeles de cada tipo y se podrá administrar la cola (script aparte)


    public void TakeReport(Component sender, object obj)
    {
        if(obj == null)
        {
            return;
        }
        GameObject reportObject = (GameObject)obj;
        reportObject.transform.SetParent(TakenPos.transform);

        changePaperInPile(reportObject);

        currentPaper = reportObject;

        SetPosition(TakenPos);
        SetPaperState(PaperState.Taken);
        reportObject.GetComponent<BoxCollider>().enabled = false;
    }


    public void OnLevePaperToPile(Component sender, object obj)
    {
        if (!currentPaper) return;

        if(actualPaperState == PaperState.HoldingRight)
        {
            TakeReport(null, currentPaper);

            OnPressButtomElement?.Invoke(this, ViewStates.OnTakenPaperView);
            return;
        }
        RefreshPaperQueue(true);
        currentPaper.transform.DOMove(ReportPilePos.position + TransformOffset, takeDuration);
        currentPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles + RotationOffset, takeDuration);
        currentPaper.GetComponent<BoxCollider>().enabled = true;
        currentPaper.transform.SetParent(ReportPilePos);
        currentPaper = null;
        SetPaperState(PaperState.Staked);
    }

    public void OnHoldPaperToButtomRigth(Component sender, object obj)
    {
        if (!currentPaper) return;
        ViewStates view = (ViewStates)obj;

        if(view != ViewStates.GeneralView && view!= ViewStates.OnTakenPaperView)
        {
            currentPaper.transform.SetParent(HoldRigthPos);
            SetPosition(HoldRigthPos);
            SetPaperState(PaperState.HoldingRight);
            currentPaper.GetComponent<BoxCollider>().enabled = true;
        }
    }

    Transform currentTarget;
    float lerpTime;
    void SetPosition(Transform target)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        currentTarget = target;
        isMoving = true;
        lerpTime = 0;

        moveSequence = DOTween.Sequence();
        moveSequence.Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, takeDuration).SetEase(Ease.InOutCirc))
               .OnComplete(() =>
               {
                        isMoving = false;
               });
    }

    private void Update()
    {
        if (isMoving && currentTarget != null && currentPaper != null)
        {
            currentPaper.transform.position = Vector3.Lerp(currentPaper.transform.position, currentTarget.position, lerpTime);
            currentPaper.transform.rotation = Quaternion.Lerp(currentPaper.transform.rotation, currentTarget.rotation, lerpTime);
        }
    }

    private void RefreshPaperQueue(bool isAdding)
    {
        if(isAdding)
        {
            if (PapersQueue.Contains(currentPaper)) return;
            if (PapersQueue.Count != 0) TransformOffset = PapersQueue.Count * new Vector3(0, 0.002f, 0);
            RotationOffset = new Vector3(0, UnityEngine.Random.Range(-10, 10), 0);
            DisableOtherPapers();
            PapersQueue.Add(currentPaper);
        }
        else
        {
            PapersQueue.Remove(currentPaper);
        }
    }

    void DisableOtherPapers()
    {
        if (PapersQueue.Count == 0) return;
        foreach(GameObject paper in PapersQueue)
        {
            if (paper == PapersQueue.Last()) return;
            paper.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void PositionOnPC(Component sender, object obj)
    {
        if (moveToPcSequence != null && moveToPcSequence.IsActive()) moveToPcSequence.Kill();

        moveToPcSequence = DOTween.Sequence();

        GameObject paperMove = currentPaper;
        currentPaper = null;

        moveToPcSequence.Append(paperMove.transform.DOMove(PCSpot1.transform.position, 0.7f).SetEase(Ease.InOutQuad))
                        .Join(paperMove.transform.DORotate(PCSpot1.transform.rotation.eulerAngles, 0.5f)
                        .OnComplete(() =>
                        {
                            moveToPcSequence.PrependInterval(0.5f)
                            .Append(paperMove.transform.DOMove(PCSpot2.transform.position, 0.3f).SetEase(Ease.InOutQuad));
                        }));
    }

    public void DescartPosition(Component sender, object obj)
    {
        if (moveDescart != null && moveDescart.IsActive()) moveDescart.Kill();

        moveDescart = DOTween.Sequence();

        GameObject paperMove = currentPaper;
        currentPaper = null;

        moveDescart.Append(paperMove.transform.DOMove(DescartPos.transform.position, 0.3f).SetEase(Ease.InBack));
    }

    public void changePaperInPile(GameObject newReport)
    {
        if (!currentPaper || actualPaperState == PaperState.HoldingRight) return;
        if (swapPapersSequence != null && swapPapersSequence.IsActive()) swapPapersSequence.Kill();

        GameObject oldPaper = currentPaper;

        swapPapersSequence = DOTween.Sequence();
        swapPapersSequence.Append(oldPaper.transform.DOMove(DescartPos.transform.position, 0.3f))
            .Append(oldPaper.transform.transform.DOMove(ReportPilePos.position + TransformOffset, takeDuration))
            .Join(oldPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles + RotationOffset, takeDuration))
            .OnComplete(() =>
            {
                oldPaper.GetComponent<BoxCollider>().enabled = true;
                oldPaper.transform.SetParent(ReportPilePos);
            }); 
    }

}
