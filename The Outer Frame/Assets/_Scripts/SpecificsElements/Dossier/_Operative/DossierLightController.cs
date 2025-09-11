using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DossierLightController : MonoBehaviour
{
    [SerializeField] Transform DefaultPosition;
    [SerializeField] Transform BoardPosition;
    Sequence moveDossierLightSequence;
    private Transform currentTarget;
    private bool isMoving;
    private float lerpTime;
    bool isInTutorial;

    public void CheckView(Component sender, object obj)
    {
        if (isInTutorial) return;
        ViewStates actualView = (ViewStates)obj;

        if (actualView == ViewStates.BoardView || actualView == ViewStates.OnTakeSomeInBoard)
        {
            MoveDossierLight(BoardPosition);
        }
        else MoveDossierLight(DefaultPosition);
    }

    void MoveDossierLight(Transform target)
    {
        if (moveDossierLightSequence.IsActive() && moveDossierLightSequence != null) moveDossierLightSequence.Kill();

        moveDossierLightSequence = DOTween.Sequence();
        currentTarget = target;
        isMoving = true;
        lerpTime = 0;

        moveDossierLightSequence
            .Append(DOTween.To(() => lerpTime, x => lerpTime = x, 1, 1f).SetEase(Ease.InQuad)).
            OnComplete(() =>
            {
                isMoving = false;
            });
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, lerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, lerpTime);
        }
    }

    public void SetIsInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;
    }
}
