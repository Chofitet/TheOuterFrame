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
    [SerializeField] Transform initPosBackDossier;
    [SerializeField] Transform PosBackDossier;
    [SerializeField] AnimationCurve OutOfBackDossierCurve;
    Animator anim;
    private Sequence moveSequence;
    private Sequence moveWithDossierSequence;
    private Sequence shakeSequence;
    private Transform currentTarget;
    private bool isMoving = false;
    private float lerpTime = 0;
    bool IsPhonesOpen;
    bool isUp;
    GameObject child;
    bool isGameOver;
    Transform OriginalTransform;
    bool dontLeaveNotebook;
    bool cancelOutView;
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
                if (isOutOfView) SetPos(7);
                dontLeaveNotebook = false;
                break;
            case ViewStates.PinchofonoView:
                SetPos(1, true, OriginalTransform, true, 0.2f);
                dontLeaveNotebook = true;
                break;
            case ViewStates.BoardView:
                if (!isOutOfView) SetPos(2);
                cancelOutView = false;
                //else SetPos(7);
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
                SetPos(5,true,null,false,0.35f);
                dontLeaveNotebook = true;
                break;
            case ViewStates.PauseView:
                SetPos(0, false);
                cancelOutView = true;
                break;
            case ViewStates.GameOverView:
                SetPos(0, false);
                isGameOver = true;
                break;
            case ViewStates.DrawerView:
                dontLeaveNotebook = false;
                SetPos(0, false);
                break;
            case ViewStates.TutorialView:
                if (IsPhonesOpen) anim.SetTrigger("close");
                if (!isOutOfView)
                {
                    if (isUp) SetPos(0, false);
                    if (dontLeaveNotebook) SetPos(6);
                    dontLeaveNotebook = false;
                }
                break;
        }
        lastView = newview;
    }

    void SetPos(int num, bool _isUp = true , Transform trans = null,  bool isPinchofono= false, float delayToGrab = 0)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        if (shakeSequence != null && shakeSequence.IsActive())
        {
            shakeSequence.Kill();
        }
        isShaking = false;
        currentTarget = Positions[num];
        isMoving = true;
        lerpTime = 0;

        float _delayToGrab = 0;
        if (!isUp) _delayToGrab = delayToGrab;

        moveSequence = DOTween.Sequence();

        if (IsPhonesOpen)
        {
            moveSequence.AppendCallback(() => CloseNotebook());
        }

        moveSequence.PrependInterval(_delayToGrab)
            .AppendCallback(()=> { SetTransform(Positions[num]); })
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, MoveDuration).SetEase(Ease.InOutQuart))
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

        if(Input.GetKeyDown(KeyCode.Mouse1) && !dontLeaveNotebook && isUp && !inputDisable)
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

    bool isShaking = false;
    public void ShakeNotebook(Component sender, object obj)
    {
        if (!isUp || isShaking) return;
        isShaking = true;

        shakeSequence = DOTween.Sequence();

        shakeSequence
            .Append(transform.DOShakeRotation(0.4f, new Vector3(0, 5, 0), 8, 90, true, ShakeRandomnessMode.Harmonic).OnComplete(() => isShaking = false))
            .Join(transform.DOShakeRotation(0.4f, new Vector3(0, 5, 0), 8, 90, true, ShakeRandomnessMode.Harmonic));
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

    bool inputDisable;
    public void DisableInput(Component sender, object obj)
    {
        inputDisable = true;
    }

    public void EnableInput(Component sender, object obj)
    {
        inputDisable = false;
    }

    public void UpdatePositionInBoardPosition(Component seender, object obj)
    {
        SetPos(2);
    }
    bool isOutOfView;
    public void NotebookIsOutOfView(Component sender, object obj)
    {
        isOutOfView = !isOutOfView;
    }

    public void AnimNotebookIsOutOfView(Component sender, object obj)
    {
        NotebookIsOutOfView(null, null);
        if(!cancelOutView) SetPos(7, true);
    }

    public void TakeNotebookWithDossier(Component sender, object obj)
    {
        if (moveWithDossierSequence != null && moveWithDossierSequence.IsActive()) moveWithDossierSequence.Kill();

        moveWithDossierSequence = DOTween.Sequence();
       // transform.position = initPosBackDossier.position;
       // transform.rotation = initPosBackDossier.rotation;
        isOutOfView = false;
        //.Append(transform.DOMove(PosBackDossier.position, 1.2f))
        moveWithDossierSequence
                                .AppendInterval(1.2f) //1f
                                .Append(transform.DOMoveZ(Positions[6].position.z, 0.6f).SetEase(OutOfBackDossierCurve))
                                .Join(transform.DOMoveX(Positions[6].position.x, 0.6f).SetEase(Ease.InOutBack))
                                .Join(transform.DOMoveY(Positions[6].position.y, 0.6f))
                                .Join(transform.DORotate(Positions[6].rotation.eulerAngles,0.6f));

    }

    [ContextMenu("Start TakeNotebookWithDossier")]
    void TestTakeNotebookWithDossier()
    {
        TakeNotebookWithDossier(null, null);
    }




}
