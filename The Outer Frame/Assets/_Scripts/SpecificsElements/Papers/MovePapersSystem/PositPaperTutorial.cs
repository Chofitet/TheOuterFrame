using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositPaperTutorial : MonoBehaviour
{
    [SerializeField] Transform FinalPosition;
    [SerializeField] Transform TakePosition;
    [SerializeField] float TakeSpeed;
    [SerializeField] AnimationCurve _animationCurve;
    [SerializeField] GameEvent OnPositTake;
    [SerializeField] bool SendSelfThroughOnPositTake = true;
    [SerializeField] GameEvent OnButtonElement;
    [SerializeField] ViewStates ViewToChange;
    [SerializeField] bool takeShorterWay = true;
    
    Sequence MoveSequence;
    private bool pendingLeave;
    private BoxCollider _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        OnTakePosIt(null, null);
        _collider.enabled = false;
    }
    public void OnTakePosIt(Component sender, object obj)
    {
        if (MoveSequence != null && MoveSequence.IsActive())
            MoveSequence.Kill();

        Vector3 startRot = transform.localEulerAngles;
        Vector3 targetRot = TakePosition.localEulerAngles;

        MoveSequence = DOTween.Sequence();
        pendingLeave = false;

        OnButtonElement?.Invoke(this, ViewToChange);
        

        // Calcular los ángulos de destino más cercanos (evita giros largos)
        float targetX = startRot.x + Mathf.DeltaAngle(startRot.x, targetRot.x);
        float targetY = startRot.y + Mathf.DeltaAngle(startRot.y, targetRot.y);
        float targetZ = startRot.z + Mathf.DeltaAngle(startRot.z, targetRot.z);

        float curX = startRot.x;
        float curY = startRot.y;
        float curZ = startRot.z;

        if(SendSelfThroughOnPositTake) OnPositTake?.Invoke(this,gameObject);
        else OnPositTake?.Invoke(this, null);

        void ApplyEuler() => transform.localEulerAngles = new Vector3(curX, curY, curZ);

        // Movimiento + rotación sincronizada
        MoveSequence.Append(transform.DOMoveX(TakePosition.position.x, TakeSpeed))
            .Join(transform.DOMoveY(TakePosition.position.y, TakeSpeed).SetEase(_animationCurve))
            .Join(transform.DOMoveZ(TakePosition.position.z, TakeSpeed));

        if (takeShorterWay)
        {
            MoveSequence.Join(DOTween.To(() => curX, x => { curX = x; ApplyEuler(); }, targetX, TakeSpeed)
            .SetEase(Ease.OutSine));
            MoveSequence.Join(DOTween.To(() => curY, y => { curY = y; ApplyEuler(); }, targetY, TakeSpeed)
                .SetEase(Ease.InQuad));
            MoveSequence.Join(DOTween.To(() => curZ, z => { curZ = z; ApplyEuler(); }, targetZ, TakeSpeed)
                .SetEase(Ease.InQuad));
        }
        else
        {
            MoveSequence.Join(transform.DORotate(TakePosition.rotation.eulerAngles, TakeSpeed));
        }

        MoveSequence.OnComplete(() =>
        {
            transform.localEulerAngles = targetRot; // Corrige cualquier pequeña desviación

            if (pendingLeave)
                OnLeavePosIt(null, null);
        });

    }

    public void OnLeavePosIt(Component sender, object obj)
    {
        // Si está en movimiento hacia arriba, marcamos que al llegar debe bajar
        if (MoveSequence != null && MoveSequence.IsActive() && !pendingLeave)
        {
            pendingLeave = true;
            return;
        }

        MoveSequence = DOTween.Sequence();

        MoveSequence.Append(transform.DOMove(FinalPosition.position, TakeSpeed))
            .Join(transform.DORotate(FinalPosition.rotation.eulerAngles, TakeSpeed))
            .OnComplete(() => _collider.enabled = true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnLeavePosIt(null, null);
        }
    }

    public void AnotherPostItUp(Component sender, object obj)
    {
        if ((GameObject)obj != gameObject) OnLeavePosIt(null, null);
    }
}
