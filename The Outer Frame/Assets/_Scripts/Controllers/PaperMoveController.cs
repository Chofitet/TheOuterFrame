using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaperMoveController : MonoBehaviour
{
    [SerializeField] Transform TakenPos;
    [SerializeField] GameObject ReportInstances;
    [SerializeField] Transform ReportPilePos;
    [SerializeField] Transform HoldRigthPos;
    [SerializeField] float takeDuration;
    [SerializeField] GameEvent OnPressButtomElement;
    [SerializeField] GameEvent OnSetPaperState;
    private bool isMoving;
    GameObject currentPaper;
    bool isHolding;
    private Sequence moveSequence;
    PaperState actualPaperState;

    enum PaperState
    {
        Nothing,
        Taken,
        HoldingRight,
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
        currentPaper.transform.DOMove(ReportPilePos.position, takeDuration);
        currentPaper.transform.DORotate(ReportPilePos.rotation.eulerAngles, takeDuration);
        currentPaper.GetComponent<BoxCollider>().enabled = true;
        currentPaper = null;
        SetPaperState(PaperState.Nothing);
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
    

}
