using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PaperMoveController : MonoBehaviour
{
    [SerializeField] Transform TakenPos;
    [SerializeField] Transform ReportPilePos;
    [SerializeField] Transform HoldRigthPos;
    [SerializeField] float takeDuration;
    [SerializeField] Transform PCSpotReport1;
    [SerializeField] Transform PCSpotReport2;
    [SerializeField] Transform PCSpotTranscription1;
    [SerializeField] Transform PCSpotTranscription2;
    [SerializeField] Transform DescartPos;
    [SerializeField] GameEvent OnPressButtomElement;
    [SerializeField] GameEvent OnSetPaperState;
    [SerializeField] GameEvent OnReportEnterDatabase;
    [SerializeField] GameEvent OnTranscriptionEnterDatabase;
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

    public void TakeHoldingReport(Component sender, object obj)
    {
        if (PaperState.HoldingRight != actualPaperState) return;
        TakeReport(null, currentPaper);
    }


    public void TakeReport(Component sender, object obj)
    {
        if(obj == null)
        {
            return;
        }

        GameObject reportObject = (GameObject)obj;
        reportObject.transform.SetParent(TakenPos.transform);

        changePaperInPile(reportObject);

        if (PaperState.HoldingRight == actualPaperState && currentPaper != reportObject)
        {
            LeavePaperPile();
        }

        currentPaper = reportObject;

        SetPosition(TakenPos);
        SetPaperState(PaperState.Taken);
        reportObject.GetComponent<BoxCollider>().enabled = false;
        EnableLastBoxCollider();
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
        LeavePaperPile();
    }

    void LeavePaperPile()
    {
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

        if(paperMove.GetComponent<IndividualReportController>())
        {
            MoToRigthSlotOnPC(paperMove, PCSpotReport1, PCSpotReport2, OnReportEnterDatabase);
        }
        else if(paperMove.GetComponent<IndividualCallController>())
        {
            MoToRigthSlotOnPC(paperMove, PCSpotTranscription1, PCSpotTranscription2, OnTranscriptionEnterDatabase);
        }
    }

    void MoToRigthSlotOnPC(GameObject paperMove, Transform PCSpot1, Transform PCSpot2, GameEvent OnEvent)
    {
        moveToPcSequence.Append(paperMove.transform.DOMove(PCSpot1.position, 0.7f).SetEase(Ease.InOutQuad))
                       .Join(paperMove.transform.DORotate(PCSpot1.rotation.eulerAngles, 0.5f)
                       .OnComplete(() =>
                       {
                           paperMove.transform.SetParent(ReportPilePos);
                           OnEvent?.Invoke(this, null);
                           moveToPcSequence.PrependInterval(0.5f)
                           .Append(paperMove.transform.DOMove(PCSpot2.position, 0.3f).SetEase(Ease.InOutQuad));
                           EnableLastBoxCollider();
                       }));
    }

    public void DescartPosition(Component sender, object obj)
    {
        if (moveDescart != null && moveDescart.IsActive()) moveDescart.Kill();

        moveDescart = DOTween.Sequence();

        GameObject paperMove = currentPaper;
        currentPaper = null;

        moveDescart.Append(paperMove.transform.DOMove(DescartPos.transform.position, 0.5f).SetEase(Ease.InBack));
        EnableLastBoxCollider();
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

    void EnableLastBoxCollider()
    {
        if (ReportPilePos.transform.childCount > 0)
        {
            Transform lastChild = ReportPilePos.transform.GetChild(ReportPilePos.transform.childCount - 1);

            BoxCollider boxCollider = lastChild.GetComponent<BoxCollider>();

            if (boxCollider != null)
            {
                boxCollider.enabled = true;
                Debug.Log("BoxCollider habilitado en: " + lastChild.name);
            }
            else
            {
                Debug.LogWarning("El último hijo no tiene un BoxCollider.");
            }
        }
        else
        {
            Debug.LogWarning("El GameObject no tiene hijos.");
        }
    }
}
