using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MoveMemberWordsToHand : MonoBehaviour
{
    //Sequence MoveMemberWordToHandSequence;
    [SerializeField] Transform FinalPos;
    [SerializeField] float AnimDownPapersGoTolevel1 = 0.5f;
    private bool isFollowingTarget;
    Transform currentTarget;
    [SerializeField] GameEvent OnTakeDownMemberWordsSound;

    bool isMoving;
    float lerpTime;

    Sequence GetOutSequence;
    public void GetOutMemberWords(Component sender,object obj)
    {
        int numberOfPapers = (int)obj;

        if (GetOutSequence != null && GetOutSequence.IsActive())
            GetOutSequence.Kill();

        currentTarget = null;
        lerpTime = 0f;
        isFollowingTarget = false;

        GetOutSequence = DOTween.Sequence();

        GetOutSequence
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                currentTarget = FinalPos;
                isFollowingTarget = true;

                PaperOutSound(numberOfPapers);
            })
            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1f,
                    AnimDownPapersGoTolevel1)
            )
            .OnComplete(() =>
            {
                isFollowingTarget = false;

                if (currentTarget != null)
                    transform.position = currentTarget.position;

                currentTarget = null;
            });
    }

    void PaperOutSound(int numberOfPapers)
    {
        if(numberOfPapers != 0)
        {
            OnTakeDownMemberWordsSound?.Invoke(this, null);
        }
    }

    private void Update()
    {
        if (!isFollowingTarget || currentTarget == null)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            currentTarget.position,
            lerpTime);
    }
}
