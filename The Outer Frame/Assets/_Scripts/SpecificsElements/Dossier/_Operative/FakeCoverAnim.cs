using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCoverAnim : MonoBehaviour
{
    [SerializeField] GameObject dossier;
    [SerializeField] Transform endPos;
    [SerializeField] float duration;
    [SerializeField] GameEvent ShakeDossier;
    bool once;
    Sequence sequence;
    public void Anim(Component sender, object obj)
    {
        if (once) return;
        sequence?.Kill();
        sequence = DOTween.Sequence();

        dossier.transform.position = transform.position;
        dossier.transform.rotation = transform.rotation;
        ShakeDossier?.Invoke(this, 0.8f);
        once = true;

        float t = 0;
        sequence.Append(DOTween.To(() => t, x => t = x, 1, duration))
            .Join(dossier.transform.DOLocalRotate(
        new Vector3(endPos.localEulerAngles.x, 0f, 0f), // solo eje X
        duration,
        RotateMode.FastBeyond360))
            .OnUpdate(() =>
            {
                dossier.transform.position = Vector3.Lerp(dossier.transform.position, endPos.position, Time.deltaTime * (1 / duration));
               
            });
    }

    public void SetIsInTutorial(Component sender , object obj)
    {
        if((bool) obj) dossier.SetActive(true);
    }
}
