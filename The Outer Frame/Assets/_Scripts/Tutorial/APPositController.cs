using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class APPositController : MonoBehaviour
{

    [SerializeField] Transform APposition;
    [SerializeField] Transform OutPosition;
    Sequence MoveOutSequence;
    bool WasInTutorial;
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void CheckView(Component sender, object obj)
    {
        ViewStates actualView = (ViewStates)obj;

        if (actualView == ViewStates.TutorialView) WasInTutorial = true;

    }

    public void AppearPosit(Component sender, object obj)
    {
         transform.GetChild(0).gameObject.SetActive(true);
    }
    public void MovePositOut(Component sender, object obj)
    {
        if (!WasInTutorial) return;
        MoveOutSequence = DOTween.Sequence(MoveOutSequence);
        MoveOutSequence.Append(transform.DOMove(OutPosition.position, 0.5f))
            .Join(transform.DORotate(OutPosition.rotation.eulerAngles, 0.5f));
    }
}
