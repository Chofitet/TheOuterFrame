using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShowIdeaTutorialPosit : MonoBehaviour
{
    [SerializeField] GameObject positConteiner;
    [SerializeField] GameObject positModel;
    [SerializeField] Transform InitialPosModel;
    [SerializeField] Transform initPos;
    [SerializeField] Transform finalPos;
    [SerializeField] Transform OutPos;
    [SerializeField] Transform OutPos2;
    [SerializeField] float timeToAppear = 0.6f;
    [SerializeField] float timeMovement = 0.2f;

    Sequence showPositSequence;
    bool PositShowed;
    bool DisableShowingPosit;
    bool inOnTakeSomeInBoarView;
    bool pendingToShowPosit;
    public void TryShowPosit(Component sender, object obj)
    {
        showPosit(timeToAppear);
    }


    void showPosit(float AwaitTime)
    {
        if (!isActive) return;
        if (PositShowed) return;
        if (DisableShowingPosit) return;
        if (showPositSequence != null && showPositSequence.IsActive()) showPositSequence.Kill();

        showPositSequence = DOTween.Sequence();

        

        showPositSequence
        .PrependInterval(AwaitTime + 0.5f)
        .AppendCallback(() =>
        {
            positConteiner.transform.position = initPos.position;
            positConteiner.transform.rotation = initPos.rotation;
            positModel.transform.localRotation = InitialPosModel.localRotation;

            positConteiner.SetActive(true);
            PositShowed = true;
        })
        .Append(positConteiner.transform.DOLocalRotate(finalPos.transform.localRotation.eulerAngles, timeMovement))
        .Join(positConteiner.transform.DOMove(finalPos.transform.position, timeMovement));
    }

    public void StopShowPosit(Component sender, object obj)
    {
        isActive = true;
        ViewStates ActualView = (ViewStates)obj;
        if (ActualView == ViewStates.OnTakeSomeInBoard) inOnTakeSomeInBoarView = true;
        else inOnTakeSomeInBoarView = false;

        if (DisableShowingPosit) return;

        if (showPositSequence != null && showPositSequence.IsActive()) showPositSequence.Kill();

        if (PositShowed)
        {
            showPositSequence = DOTween.Sequence();
            positConteiner.SetActive(true);
            showPositSequence.Append(positModel.transform.DOLocalRotate(new Vector3(0, 90, 60), 0.42f))
            .Join(positConteiner.transform.DOMove(OutPos.transform.position, 0.42f))
            .OnComplete(() =>
            {
                positConteiner.transform.position = initPos.position;
                positConteiner.transform.rotation = initPos.rotation;
                positModel.transform.localRotation = InitialPosModel.localRotation;
                if (pendingToShowPosit && inOnTakeSomeInBoarView)
                {
                    pendingToShowPosit = false;
                    PositShowed = false;
                    showPosit(timeToAppear - 0.35f);
                }
            })
            .AppendInterval(0.1f)
            .AppendCallback(() => positConteiner.SetActive(false));
        }
        else
        {
           positConteiner.SetActive(false);

        }

       if(!inOnTakeSomeInBoarView) PositShowed = false;
       else pendingToShowPosit = true;
    }

    public void StopShowPositWhileAPisUp(Component sender, object obj)
    {
        if (showPositSequence != null && showPositSequence.IsActive()) showPositSequence.Kill();

        if (PositShowed)
        {
            showPositSequence = DOTween.Sequence();
            positConteiner.SetActive(true);
            showPositSequence.Append(positModel.transform.DOLocalRotate(new Vector3(0, 90, 60), 0.42f))
            .Join(positConteiner.transform.DOMove(OutPos2.transform.position, 0.42f))
            .OnComplete(() =>
            {
                positConteiner.transform.position = initPos.position;
                positModel.transform.localRotation = InitialPosModel.localRotation;
                positConteiner.SetActive(false);
            });
        }
        else
        {
            positConteiner.SetActive(false);
        }

        PositShowed = false;
        DisableShowingPosit = true;
    }

    bool isActive = true;
    public void DisablePosit(Component sender, object obj)
    {
        isActive = false;
    }
}
