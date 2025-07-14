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

        currentTarget = Position3;
        isLerping = true;
        lerpTime = 0f;

        // Parte 1: Lerp a Position3
        var lerpTween = DOTween.To(() => lerpTime, x => lerpTime = x, 1f, 0.5f)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() =>
            {
                isLerping = false;
                lerpTime = 0f;
                currentTarget = null;
            });

        // Parte 2: Espera y sube dossier
        var delayAndDossierSequence = DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(() => OnDoDossierUpForAPplacing?.Invoke(this, null))
            .AppendInterval(1.2f)
            .AppendCallback(() =>
            {
                currentTarget = Position4;

            // Parte 3: Movimiento final
            var finalMoveSequence = DOTween.Sequence();
                finalMoveSequence.Append(transform.DOMoveX(currentTarget.position.x, 0.5f).SetEase(Ease.OutSine))
                                 .Join(transform.DOMoveY(currentTarget.position.y, 0.5f).SetEase(Ease.InCubic))
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

        // Encadenar todo en una sola secuencia para asegurar el orden
        APpileMuveSequence.Append(lerpTween)
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
