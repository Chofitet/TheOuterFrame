using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MoveMemberWordsToHand : MonoBehaviour
{
    [SerializeField] List<Transform> Pivots = new List<Transform>();
    //Sequence MoveMemberWordToHandSequence;
    [SerializeField] float MoveDuration = 0.3f;
    [SerializeField] Transform FinalPos;
    [SerializeField] float AnimDownPapersGoTolevel1 = 0.5f;
    private bool isFollowingTarget;
    Transform currentTarget;
    GameObject currentMemberWord;

    bool isMoving;
    float lerpTime;

    Sequence GetOutSequence;
    public void GetOutMemberWords(Component sender,object obj)
    {
        GetOutSequence = DOTween.Sequence();

            GetOutSequence
        .PrependInterval(0.5f)
        .AppendCallback(() =>
        {
            isFollowingTarget = true;
            currentTarget = FinalPos;
        })
        .AppendInterval(AnimDownPapersGoTolevel1)
        .OnComplete(() =>
        {
            isFollowingTarget = false;

            if (currentTarget != null)
                transform.position = currentTarget.position;
        });
    }

    private void Update()
    {
        if (isFollowingTarget && currentTarget != null)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                currentTarget.position,
                0.2f);
        }
    }
}
