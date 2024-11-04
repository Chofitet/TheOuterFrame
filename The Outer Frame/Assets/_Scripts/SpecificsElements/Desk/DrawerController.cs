using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class DrawerController : MonoBehaviour
{
    [SerializeField] GameEvent DisableEnvent;
    [SerializeField] GameEvent EnableInput;
    [SerializeField] Transform EndPos;
    [SerializeField] float OpenDuration;
    [SerializeField] float WaitDuration;
    [SerializeField] GameEvent BackToGeneralView;
    [SerializeField] GameEvent OnuttonElement;
    [SerializeField] Transform HoldPos;
    [SerializeField] Transform PlaceInDrawer;
    [SerializeField] GameEvent OnButtonElementClick;
    [SerializeField] GameEvent OnDrawerOpenSound;
    [SerializeField] GameEvent OnDrawerCloseSound;

    Vector3 StartPos;
    Sequence OpenDrawerSequence;
    Sequence MovePhotoSequehnce;

    private void Start()
    {
        StartPos = transform.position;
    }

    public void TakePhotoAnim(Component sender, object obj)
    {
        GameObject PhotoObject = (GameObject)obj;

        DisableEnvent?.Invoke(this, null);

        PhotoObject.transform.SetParent(HoldPos);

        MovePhotoSequehnce?.Kill();
        MovePhotoSequehnce = DOTween.Sequence();

        MovePhotoSequehnce.Append(PhotoObject.transform.DOMove(HoldPos.position, 3f))
                          .Join(PhotoObject.transform.DOScale(new Vector3(0.00278860074f, 0.0041632615f, 0.00155140401f),3f))
                          .Join(PhotoObject.transform.DORotate(HoldPos.rotation.eulerAngles, 3f))
                          .AppendInterval(1)
                          .AppendCallback(() =>
                          {
                              OnButtonElementClick?.Invoke(this, ViewStates.DrawerView);
                              OpenDrawerAnim();
                          })
                          .AppendInterval(0.5f)
                          .Append(PhotoObject.transform.DOMove(PlaceInDrawer.position, 0.3f)).SetEase(Ease.OutCirc)
                          .Join(PhotoObject.transform.DORotate(PlaceInDrawer.rotation.eulerAngles, 0.3f))
                          .AppendCallback(() =>
                          {
                              PhotoObject.transform.SetParent(gameObject.transform);
                          });


    }

    public void OpenDrawerAnim()
    {
        OpenDrawerSequence = DOTween.Sequence();
        OnDrawerOpenSound?.Invoke(this, null);

        OpenDrawerSequence.Append(transform.DOMove(EndPos.position, OpenDuration).SetEase(Ease.OutQuint))
                          .AppendInterval(WaitDuration)
                          .AppendCallback(() =>
                          {
                              OnDrawerCloseSound?.Invoke(this, null);
                              BackToGeneralView?.Invoke(this, null);
                          })
                          .Append(transform.DOMove(StartPos, OpenDuration).SetEase(Ease.OutQuint))
                          .OnComplete(()=>
                          {
                              EnableInput?.Invoke(this, null);
                          });
    }

   

}
