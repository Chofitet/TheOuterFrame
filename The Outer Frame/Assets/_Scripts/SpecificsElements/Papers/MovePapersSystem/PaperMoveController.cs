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
    [SerializeField] Transform PaperOutBoardPos;
    [SerializeField] GameEvent OnPressButtomElement;
    [SerializeField] GameEvent OnSetPaperState;
    [SerializeField] GameEvent OnReportEnterDatabase;
    [SerializeField] GameEvent OnTranscriptionEnterDatabase;
    [SerializeField] AnimationCurve ReportToButtomRigthCurve;

    [SerializeField] GameObject TutorialPosIt;
    bool AtLeastOnePaperSolved;
    bool isACandy = false;
    private bool isMoving;
    GameObject currentPaper;
    bool isHolding;
    private Sequence moveSequence;
    private Sequence moveToPcSequence;
    private Sequence moveDescart;
    private Sequence swapPapersSequence;
    private Sequence LeaveInPileOtView;
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

    bool auxComesFromRightPos = false;
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
            LeavePaperPileOutView(null,null);
        }

        if (auxComesFromRightPos && isMoving) auxComesFromRightPos = true;
        else
        {
            auxComesFromRightPos = (actualPaperState == PaperState.HoldingRight) ? true : false;
        }

        currentPaper = reportObject;
        currentPaper.GetComponent<PaperStatesController>().SetPaperState(PaperState.Taken);

        SetPosition(TakenPos);
        if(!auxComesFromRightPos) SetPosition(TakenPos);
        else SetPosition(TakenPos, Ease.OutSine, 1.5f);
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
        if(actualPaperState == PaperState.HoldingRight && !isACandy)
        {
            TakeReport(null, currentPaper);

            OnPressButtomElement?.Invoke(this, ViewStates.OnTakenPaperView);
            return;
        }
        isACandy = false;

        LeavePaperPile(null,null);
       
    }
    bool isChangingPapers;
    public void LeavePaperPile(Component sender, object obj)
    {

        if (!currentPaper) return;
        currentPaper.GetComponent<PaperStatesController>().SetPaperState(PaperState.Staked);
        currentPaper.transform.SetParent(ReportPilePos);
        int papersInQueue = RefreshPaperQueue();
        currentPaper.transform.DOMove(ReportPilePos.position + TransformOffset, takeDuration);
        currentPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles + RotationOffset, takeDuration);
        currentPaper.GetComponent<BoxCollider>().enabled = true;
        currentPaper = null;
        SetPaperState(PaperState.Nothing);

        if(!AtLeastOnePaperSolved && papersInQueue >= 3)
        {
            TutorialPosIt.SetActive(true);
        }
        EnableLastBoxCollider();
    }

    public void OnTakeCandy(Component sender, object obj)
    {
        isACandy = true;
        LeavePaperPileOutView(null, null);
    }

    public void LeavePaperPileOutView(Component sender, object obj)
    {
        if (LeaveInPileOtView != null && LeaveInPileOtView.IsActive()) LeaveInPileOtView.Kill();

        LeaveInPileOtView = DOTween.Sequence();

        if (!currentPaper) return;
        currentPaper.GetComponent<PaperStatesController>().SetPaperState(PaperState.Staked);
        currentPaper.transform.SetParent(ReportPilePos);
        RefreshPaperQueue();
        LeaveInPileOtView.Append(currentPaper.transform.DOMove(DescartPos.position, takeDuration))
            .Append(currentPaper.transform.DOMove(ReportPilePos.position + TransformOffset, takeDuration))
           .Join(currentPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles + RotationOffset, takeDuration));
        currentPaper.GetComponent<BoxCollider>().enabled = true;
        currentPaper = null;
        SetPaperState(PaperState.Nothing);
        EnableLastBoxCollider();
    }

    

    public void OnHoldPaperToButtomRigth(Component sender, object obj)
    {
        if (!currentPaper) return;
        ViewStates view = (ViewStates)obj;
        Transform auxTrans = HoldRigthPos;
        if (view == ViewStates.BoardView || view == ViewStates.OnTakeSomeInBoard) auxTrans = PaperBoardPos;
        if (view == ViewStates.BoardZoomView) auxTrans = PaperOutBoardPos;
        if (view != ViewStates.GeneralView && view!= ViewStates.OnTakenPaperView)
        {
            currentPaper.transform.SetParent(auxTrans);
            SetPosition(auxTrans, Ease.InCubic);
            SetPaperState(PaperState.HoldingRight);
            currentPaper.GetComponent<BoxCollider>().enabled = true;
        }
    }

    Transform currentTarget;
    float lerpTime;
    void SetPosition(Transform target, Ease easy = Ease.InOutCirc, float speedMove = 0.8f)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();


        currentTarget = target;
        isMoving = true;
        lerpTime = 0;

        Vector3 positionOutProgressor = currentPaper.transform.position;
        float timeToOutProgressor = 0;
        if (currentPaper.GetComponent<ReportController>() != null)
        { 
            timeToOutProgressor = 0.3f;
            positionOutProgressor = currentPaper.GetComponent<ReportController>().GetOutPos();
        }

        moveSequence = DOTween.Sequence();
        moveSequence
            .Append(currentPaper.transform.DOMove(positionOutProgressor, timeToOutProgressor).SetEase(Ease.OutCirc))
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, speedMove).SetEase(easy))
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

    private int RefreshPaperQueue()
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
            RotationOffset = new Vector3(0, UnityEngine.Random.Range(-5, 5), 0);
            
       return stakedCount;
    }


    public void PositionOnPC(Component sender, object obj)
    {
        if (moveToPcSequence != null && moveToPcSequence.IsActive()) moveToPcSequence.Kill();

        moveToPcSequence = DOTween.Sequence();
        AtLeastOnePaperSolved = true;
        TutorialPosIt.SetActive(false);
        GameObject paperMove = currentPaper;
        if (!paperMove) return;
        currentPaper = null;
        paperMove.transform.SetParent(transform);
        if (paperMove.GetComponent<IndividualReportController>())
        {
            MoToRigthSlotOnPC(paperMove, PCSpotReport1, PCSpotReport2, OnReportEnterDatabase);
        }
        else if(paperMove.GetComponent<IndividualCallController>())
        {
            MoToRigthSlotOnPC(paperMove, PCSpotTranscription1, PCSpotTranscription2, OnTranscriptionEnterDatabase);
        }
        EnableLastBoxCollider();
    }

    void MoToRigthSlotOnPC(GameObject paperMove, Transform PCSpot1, Transform PCSpot2, GameEvent OnEvent)
    {
        
        moveToPcSequence.Append(paperMove.transform.DOMove(PCSpot1.position, 0.7f).SetEase(Ease.InOutQuad))
                       .Join(paperMove.transform.DORotate(PCSpot1.rotation.eulerAngles, 0.5f)
                       .OnComplete(() =>
                       {
                           paperMove.transform.SetParent(transform);
                           OnEvent?.Invoke(this, null);
                           moveToPcSequence.PrependInterval(0.5f)
                           .Append(paperMove.transform.DOMove(PCSpot2.position, 0.3f).SetEase(Ease.InOutQuad));
                           
                       }));
    }

    public void DescartPosition(Component sender, object obj)
    {
        if (moveDescart != null && moveDescart.IsActive()) moveDescart.Kill();
        AtLeastOnePaperSolved = true;
        TutorialPosIt.SetActive(false);
        moveDescart = DOTween.Sequence();

        GameObject paperMove = currentPaper;
        if (!paperMove) return;
        currentPaper = null;
        paperMove.transform.SetParent(transform);
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
                EnableLastBoxCollider();
            });
       
    }

    void EnableLastBoxCollider()
    {
        int childCount = ReportPilePos.transform.childCount;

        if (childCount == 0)
            return;

        // Desactiva todos los BoxColliders primero
        for (int i = 0; i < childCount; i++)
        {
            Transform child = ReportPilePos.transform.GetChild(i);
            BoxCollider collider = child.GetComponent<BoxCollider>();
            if (collider != null)
                collider.enabled = false;
        }

        // Activa solo el último
        Transform lastChild = ReportPilePos.transform.GetChild(childCount - 1);
        BoxCollider lastCollider = lastChild.GetComponent<BoxCollider>();
        if (lastCollider != null)
        {
            lastCollider.enabled = true;
            // Debug.Log("BoxCollider habilitado en: " + lastChild.name);
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
