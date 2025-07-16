using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class APpileController : MonoBehaviour
{
    Sequence APpileMuveSequence;
    [SerializeField] Transform Position1;
    [SerializeField] Transform Position2;
    [SerializeField] Transform Position3;
    [SerializeField] Transform Position4;
    [SerializeField] Transform UltimaPosition;
    [SerializeField] Transform CamParent;
    [SerializeField] GameEvent OnAPpilePlaced;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDoDossierUpForAPplacing;
    bool inAPTaking;

    float lerpTime; // Asegurate de tener esta como variable de clase
    bool isLerping;
    Transform currentTarget;

    public void APpileMove(Component sender, object obj)
    {
        if (APpileMuveSequence != null && APpileMuveSequence.IsActive()) APpileMuveSequence.Kill();

        APpileMuveSequence = DOTween.Sequence();
        inAPTaking = true;

        OnDisableInput?.Invoke(this, null);
        currentTarget = Position2;
        isLerping = true;
        currentTarget = Position2;
        transform.SetParent(CamParent);

        APpileMuveSequence.Append(transform.DOMove(Position1.position, 0.15f)).SetEase(Ease.InSine)
                            .Join(transform.DORotate(Position1.rotation.eulerAngles, 0.15f))
                            .Join(transform.DOScale(Vector3.one, 0.7f))
                            .Join(DOTween.To(() => lerpTime, x => lerpTime = x, 1f, 1.5f)
                          .SetEase(Ease.InOutQuart));
                          
                         
    }

    public void APpileSecondPart(Component sender, object obj)
    {
        if (!inAPTaking) return;
        if (APpileMuveSequence != null && APpileMuveSequence.IsActive()) APpileMuveSequence.Kill();

        APpileMuveSequence = DOTween.Sequence();

        // Primer Lerp: hacia Position3
        currentTarget = Position3;
        isLerping = true;
        lerpTime = 0f;

        var lerpToPosition3 = DOTween.To(() => lerpTime, x => lerpTime = x, 1f, 0.5f)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() =>
            {
                isLerping = false;
                lerpTime = 0f;
                currentTarget = null;
            });

        // Segundo Lerp: hacia Position4

        Tween lerpToPosition4 = DOTween.Sequence();

        var delayAndDossierSequence = DOTween.Sequence()
            .AppendInterval(1)
            .AppendCallback(() => OnDoDossierUpForAPplacing?.Invoke(this, null))
            .AppendCallback(() =>
            {
                lerpToPosition4 = DOTween.Sequence()
                .AppendInterval(0.4f)
                .AppendCallback(() =>
                {
                    currentTarget = Position4;
                    isLerping = true;
                    lerpTime = 0f;
                })
                .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1f, 0.7f)
                    .SetEase(Ease.InOutQuart)
                    .OnComplete(() =>
                    {
                        isLerping = false;
                        lerpTime = 0f;
                        currentTarget = null;
                    })
            );})
            .AppendInterval(1.2f)
            .AppendCallback(() =>
            {
                currentTarget = UltimaPosition;

                var finalMoveSequence = DOTween.Sequence();
                finalMoveSequence.Append(transform.DOMoveX(currentTarget.position.x, 0.5f).SetEase(Ease.OutSine))
                                 .Join(transform.DOMoveY(currentTarget.position.y, 0.5f))
                                 .Join(transform.DOMoveZ(currentTarget.position.z, 0.5f))
                                 .Join(transform.DORotate(currentTarget.rotation.eulerAngles, 0.5f).SetEase(Ease.OutCubic))
                                 .OnComplete(() =>
                                 {
                                     isLerping = false;
                                     OnAPpilePlaced?.Invoke(this, null);
                                     OnEnableInput?.Invoke(this, null);
                                     inAPTaking = false;
                                     Destroy(gameObject);
                                 });
            });

        APpileMuveSequence.Append(lerpToPosition3)
                          .Append(lerpToPosition4)
                          .Append(delayAndDossierSequence);
    }

    void Update()
    {
        if (isLerping && currentTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, lerpTime);
        }
    }
}
