using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NotebookMoveController : MonoBehaviour
{
    [SerializeField] Transform[] Positions;
    [SerializeField] float MoveDuration;
    Animator anim;
    private Sequence moveSequence;
    private Sequence moveWiteWordSequence;
    private Transform currentTarget;
    private bool isMoving = false;
    private float lerpTime = 0;
    bool IsPhonesOpen;

    ViewStates lastView;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnChangeView(Component sender, object obj)
    {
        ViewStates newview = (ViewStates)obj;
        switch (newview)
        {
            case ViewStates.GeneralView:
                SetPos(0);
                break;
            case ViewStates.DossierView:
                SetPos(1);
                break;
            case ViewStates.PinchofonoView:
                SetPos(1, true);
                break;
            case ViewStates.BoardView:
                SetPos(1);
                break;
            case ViewStates.PCView:
                SetPos(1);
                break;
            case ViewStates.ProgressorView:
                SetPos(0);
                break;
            case ViewStates.OnCallTranscriptionView:
                SetPos(1);
                break;
            case ViewStates.TVView:
                SetPos(1);
                break;
            case ViewStates.PrinterView:
                SetPos(1);
                break;
        }
        lastView = newview;
    }

    void SetPos(int num, bool isPinchofono= false)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        Ease ease = lastView == ViewStates.DossierView && num != 0 ? Ease.Linear : Ease.InOutCirc;
        currentTarget = Positions[num];
        isMoving = true;
        lerpTime = 0;

        moveSequence = DOTween.Sequence();

        if (IsPhonesOpen)
        {
            moveSequence.AppendCallback(() => CloseNotebook());
        }

        moveSequence.PrependInterval(0.1f);
        moveSequence.Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, MoveDuration).SetEase(ease))
                    .OnComplete(() =>
                    {
                        isMoving = false;
                        if (isPinchofono)
                        {
                            OpenPhoneNums();
                        }
                    });
    }

    private void Update()
    {
        if (isMoving && currentTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, lerpTime);
        }
    }

    
    public void WriteWord(Component sender, object obj)
    {
        WordData word = (WordData)obj;


        //WriteWordAnim(word.GetIsAPhoneNumber());
    }

    /* Animacion de bajar y subir la libreta
    void WriteWordAnim(bool isWordPhoneNum)
    {
        if (moveWiteWordSequence != null && moveWiteWordSequence.IsActive()) moveWiteWordSequence.Kill();

        moveWiteWordSequence = DOTween.Sequence();
        moveWiteWordSequence.Append(transform.DOMove(Positions[2].position, 0.5f).SetEase(Ease.InBack)).Join(transform.DORotate(Positions[2].rotation.eulerAngles,0.5f).SetEase(Ease.InSine))
            .AppendCallback(() =>
            {
                if (IsPhonesOpen && !isWordPhoneNum) SlidePhones(null, null);
                if (!IsPhonesOpen && isWordPhoneNum) SlidePhones(null, null);
            })
            .AppendInterval(0.3f)
            .Append(transform.DOMove(Positions[1].position, 0.5f).SetEase(Ease.OutBack)).Join(transform.DORotate(Positions[1].rotation.eulerAngles, 0.5f).SetEase(Ease.OutSine));
    }*/

    public void SlidePhones(Component sender, object obj)
    {

        if (obj is bool)
        {
            bool toOpenPhones = (bool)obj;
            if (!IsPhonesOpen && toOpenPhones) OpenPhoneNums();
            else if(IsPhonesOpen && !toOpenPhones)
            {
                CloseNotebook();
            }
            return;
        }
        if (!IsPhonesOpen) OpenPhoneNums();
        else
        {
            CloseNotebook();
        }
    }

    public void ShakeNotebook(Component sender, object obj)
    {
        transform.DOShakeRotation(0.4f, new Vector3(0,5,0),8,90,true,ShakeRandomnessMode.Harmonic);
    }

    void OpenPhoneNums()
    {
        anim.SetTrigger("open");
        IsPhonesOpen = true;
    }

    void CloseNotebook()
    {
        anim.SetTrigger("close");
        IsPhonesOpen = false;
    }

}
