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
                SetPos(0);
                break;
        }
        lastView = newview;
    }

    void SetPos(int num, bool isPinchofono= false)
    {
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }

        currentTarget = Positions[num];
        isMoving = true;
        lerpTime = 0;

        moveSequence = DOTween.Sequence();

        if (IsPhonesOpen)
        {
            moveSequence.AppendCallback(() => CloseNotebook());
        }

        moveSequence.PrependInterval(0.1f);
        moveSequence.Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, MoveDuration).SetEase(Ease.InOutCirc))
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

    public void SlidePhones(Component sender, object obj)
    {
        if(!IsPhonesOpen) OpenPhoneNums(); 
        else CloseNotebook();
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
