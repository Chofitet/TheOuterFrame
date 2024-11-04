using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NotebookMoveController : MonoBehaviour
{
    [SerializeField] Transform[] Positions;
    [SerializeField] float MoveDuration;
    [SerializeField] Transform VanimPos;
    [SerializeField] GameEvent OnSlidePhoneUpSound;
    [SerializeField] GameEvent OnSlidePhoneDownSound;
    [SerializeField] GameEvent OnTakeNotebookSound;
    [SerializeField] GameEvent OnLeaveNotebookSound;
    [SerializeField] GameEvent OnManualSlidePhonePage;
    Animator anim;
    private Sequence moveSequence;
    private Sequence moveWiteWordSequence;
    private Transform currentTarget;
    private bool isMoving = false;
    private float lerpTime = 0;
    bool IsPhonesOpen;
    bool isUp;
    GameObject child;
    bool isGameOver;
    Transform OriginalTransform;
    bool dontLeaveNotebook;

    ViewStates lastView;

    private void Start()
    {
        child = transform.GetChild(0).gameObject;
        anim = child.GetComponent<Animator>();
        OriginalTransform = transform.parent;
    }



    public void OnChangeView(Component sender, object obj)
    {
        //anim.Play("notebook armature|idle");
        if (isGameOver) return;
        ViewStates newview = (ViewStates)obj;
        switch (newview)
        {
            case ViewStates.GeneralView:
                if(IsPhonesOpen) anim.SetTrigger("close");
                if (isUp) SetPos(0, false);
                if (dontLeaveNotebook) SetPos(6);
                dontLeaveNotebook = false;
                break;
            case ViewStates.DossierView:
                SetPos(6);
                dontLeaveNotebook = false;
                break;
            case ViewStates.PinchofonoView:
                SetPos(1, true, OriginalTransform, true);
                dontLeaveNotebook = true;
                break;
            case ViewStates.BoardView:
                SetPos(2);
                dontLeaveNotebook = true;
                break;
            case ViewStates.PCView:
                SetPos(3);
                dontLeaveNotebook = true;
                break;
            case ViewStates.ProgressorView:
                if(isUp) SetPos(0,false);
                break;
            case ViewStates.OnTakenPaperView:
                SetPos(6);
                dontLeaveNotebook = true;
                break;
            case ViewStates.TVView:
                SetPos(5);
                dontLeaveNotebook = true;
                break;
            case ViewStates.PauseView:
                SetPos(0, false);
                break;
            case ViewStates.GameOverView:
                SetPos(0, false);
                isGameOver = true;
                break;
            case ViewStates.DrawerView:
                dontLeaveNotebook = false;
                SetPos(0, false);
                break;
        }
        lastView = newview;
    }

    void SetPos(int num, bool _isUp = true , Transform trans = null,  bool isPinchofono= false)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        currentTarget = Positions[num];
        isMoving = true;
        lerpTime = 0;

        SetTransform(Positions[num]);

        moveSequence = DOTween.Sequence();

        if (IsPhonesOpen)
        {
            moveSequence.AppendCallback(() => CloseNotebook());
        }

        //moveSequence.PrependInterval(0.1f);
        moveSequence.Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, MoveDuration).SetEase(Ease.InOutCirc))
                    .OnComplete(() =>
                    {
                        isMoving = false;
                        if (isPinchofono)
                        {
                            OpenPhoneNums();
                        }
                        child.transform.DOLocalRotate(Vector3.zero, 0.2f);
                        SetTransform(OriginalTransform);
                    });
        
        isUp = _isUp;

        if (dontLeaveNotebook) return;
        if (isUp) OnTakeNotebookSound?.Invoke(this, null);
        else OnLeaveNotebookSound?.Invoke(this, null);
    }

    private void Update()
    {
        if (isMoving && currentTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, lerpTime);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && !dontLeaveNotebook && isUp)
        {
            SetPos(0, false);
        }
    }

    void SetTransform(Transform trans)
    {
        if (trans == null) return;
        transform.SetParent(trans);
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
        OnManualSlidePhonePage?.Invoke(this, null);
        if (!IsPhonesOpen) OpenPhoneNums();
        else
        {
            CloseNotebook();
        }
    }

    public void ShakeNotebook(Component sender, object obj)
    {
        transform.DOShakeRotation(0.4f, new Vector3(0,5,0),8,90,true,ShakeRandomnessMode.Harmonic);
        transform.DOShakeRotation(0.4f, new Vector3(0, 5, 0), 8, 90, true, ShakeRandomnessMode.Harmonic);
    }

    void OpenPhoneNums()
    {
        if (IsPhonesOpen) return;
        anim.SetTrigger("open");
        anim.ResetTrigger("close");
        IsPhonesOpen = true;
        OnSlidePhoneUpSound?.Invoke(this, null);
    }

    void CloseNotebook()
    {
        if (!IsPhonesOpen) return;
        anim.SetTrigger("close");
        anim.ResetTrigger("open");
        IsPhonesOpen = false;
        OnSlidePhoneDownSound?.Invoke(this, null);
    }

    public void CompleteSlotsProgressorAnim(Component sender, object obj)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();
        moveSequence = DOTween.Sequence();

        moveSequence.Append(transform.DOMove(VanimPos.position, 0.2f))
            .Join(transform.DORotate(VanimPos.rotation.eulerAngles,0.2f))
            .AppendInterval(0.4f)
            .Append(transform.DOMove(Positions[6].position, 0.2f))
            .Join(transform.DORotate(Positions[6].rotation.eulerAngles, 0.2f));

    }

    public void LeaveNotebook(Component sender, object obj)
    {
        SetPos(0, false);
    }

}
