using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class APpileController : MonoBehaviour
{
    Sequence APpileMuveSequence;
    [SerializeField] Transform Position1;
    [SerializeField] Transform Position2;
    [SerializeField] GameEvent OnAPpilePlaced;

    public void APpileMove(Component sender, object obj)
    {
        if (APpileMuveSequence != null && APpileMuveSequence.IsActive()) APpileMuveSequence.Kill();

        APpileMuveSequence = DOTween.Sequence();

        APpileMuveSequence.Append(transform.DOMove(Position1.position, 1))
                            .Join(transform.DORotate(Position1.rotation.eulerAngles, 1))
                            .Append(transform.DOMove(Position2.position, 0.6f))
                            .Join(transform.DORotate(Position2.rotation.eulerAngles, 0.6f))
                            .OnComplete(() =>
                            {
                                OnAPpilePlaced?.Invoke(this, null);
                                gameObject.SetActive(false);
                            });

    }
}
