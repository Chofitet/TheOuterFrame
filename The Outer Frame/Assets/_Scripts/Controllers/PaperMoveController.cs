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
    [SerializeField] Transform ReportPilePos2;
    [SerializeField] Transform HoldRigthPos;
    [SerializeField] float takeDuration;
    [SerializeField] Transform PCSpotReport1;
    [SerializeField] Transform PCSpotReport2;
    [SerializeField] Transform PCSpotTranscription1;
    [SerializeField] Transform PCSpotTranscription2;
    [SerializeField] Transform DescartPos;
    [SerializeField] Transform PaperBoardPos;
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

    public enum PaperState
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
        if (isChangingPapers) return;
        if(obj == null)
        {
            return;
        }


        GameObject reportObject = (GameObject)obj;
        reportObject.transform.SetParent(TakenPos.transform);
        

        if (currentPaper != reportObject) changePaperInPile(reportObject);

        if (PaperState.HoldingRight == actualPaperState && currentPaper != reportObject)
        {
            LeavePaperPile(null,null);
        }

        currentPaper = reportObject;
        currentPaper.GetComponent<PaperStatesController>().SetPaperState(PaperState.Taken);

        SetPosition(TakenPos);
        SetPaperState(PaperState.Taken);
        reportObject.GetComponent<BoxCollider>().enabled = false;
        EnableLastBoxCollider();
    }


    public void OnLevePaperToPile(Component sender, object obj)
    {
        if (!currentPaper) return;
        if (currentPaper.GetComponent<IndividualReportController>())
        {
            if (currentPaper.GetComponent<IndividualReportController>().GetRepoertype().GetDeleteDBRepoert()) return;
        }
        if(actualPaperState == PaperState.HoldingRight)
        {
            TakeReport(null, currentPaper);

            OnPressButtomElement?.Invoke(this, ViewStates.OnTakenPaperView);
            return;
        }
        LeavePaperPile(null,null);
       
    }
    bool isChangingPapers;
    public void LeavePaperPile(Component sender, object obj)
    {

        if (!currentPaper) return;
        currentPaper.GetComponent<PaperStatesController>().SetPaperState(PaperState.Staked);
        currentPaper.transform.SetParent(ReportPilePos);
        RefreshPaperQueue();
        currentPaper.transform.DOMove(ReportPilePos.position + TransformOffset, takeDuration);
        currentPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles + RotationOffset, takeDuration);
        currentPaper.GetComponent<BoxCollider>().enabled = true;
        currentPaper = null;
        SetPaperState(PaperState.Nothing);

    }

    public void OnHoldPaperToButtomRigth(Component sender, object obj)
    {
        if (!currentPaper) return;
        ViewStates view = (ViewStates)obj;
        Transform auxTrans = HoldRigthPos;
        if (view == ViewStates.BoardView || view == ViewStates.OnTakeSomeInBoard) auxTrans = PaperBoardPos;

        if (view != ViewStates.GeneralView && view!= ViewStates.OnTakenPaperView)
        {
            currentPaper.transform.SetParent(auxTrans);
            SetPosition(auxTrans);
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

    private void RefreshPaperQueue()
    {
        int stakedCount = 0;

        foreach (Transform child in ReportPilePos.transform)
        {
            PaperStatesController paperController = child.GetComponent<PaperStatesController>();
            if (paperController != null && paperController.GetPaperState() == PaperState.Staked)
            {
                stakedCount++;
            }
        }

            TransformOffset = stakedCount * new Vector3(0, 0.002f, 0);
            RotationOffset = new Vector3(0, UnityEngine.Random.Range(-10, 10), 0);
            
       
    }


    public void PositionOnPC(Component sender, object obj)
    {
        if (moveToPcSequence != null && moveToPcSequence.IsActive()) moveToPcSequence.Kill();

        moveToPcSequence = DOTween.Sequence();

        GameObject paperMove = currentPaper;
        if (!paperMove) return;
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
        if (!paperMove) return;
        currentPaper = null;

        moveDescart.Append(paperMove.transform.DOMove(DescartPos.transform.position, 0.5f).SetEase(Ease.InBack));
        EnableLastBoxCollider();
    }
    public void changePaperInPile(GameObject newReport)
    {
        if (!currentPaper || actualPaperState == PaperState.HoldingRight) return;
        if (swapPapersSequence != null && swapPapersSequence.IsActive()) swapPapersSequence.Kill();

        GameObject oldPaper = currentPaper;
        isChangingPapers = true;

        oldPaper.transform.SetParent(ReportPilePos);
        oldPaper.GetComponent<PaperStatesController>().SetPaperState(PaperState.Staked);
        RefreshPaperQueue();
        swapPapersSequence = DOTween.Sequence();
        swapPapersSequence.Append(oldPaper.transform.DOMove(ReportPilePos2.transform.position, 0.3f))
            .Append(oldPaper.transform.transform.DOMove(ReportPilePos.position + TransformOffset, takeDuration))
            .Join(oldPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles + RotationOffset, takeDuration))
            .OnComplete(() =>
            {
                oldPaper.GetComponent<BoxCollider>().enabled = true;
                
                isChangingPapers = false;
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
                Debug.LogWarning("El �ltimo hijo no tiene un BoxCollider.");
            }
        }
        else
        {
            Debug.LogWarning("El GameObject no tiene hijos.");
        }
    }

    public void DisableAllPapersInPile(Component sender, object obj)
    {
        foreach (Transform child in ReportPilePos.transform)
        {
            PaperStatesController paperController = child.GetComponent<PaperStatesController>();
            if (paperController != null && paperController.GetPaperState() == PaperState.Staked)
            {
                child.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public void DeleteAllPapersInPile(Component sender, object obj)
    {
        foreach (Transform child in ReportPilePos.transform)
        {
            PaperStatesController paperController = child.GetComponent<PaperStatesController>();
            if (paperController != null && paperController.GetPaperState() == PaperState.Staked)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
