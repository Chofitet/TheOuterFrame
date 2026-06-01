using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MoveMemberWordsToHand : MonoBehaviour
{
    [SerializeField] List<Transform> Pivots = new List<Transform>();
    Sequence MoveMemberWordToHandSequence;
    [SerializeField] float MoveDuration = 0.3f;

    Transform currentTarget;
    GameObject currentMemberWord;

    bool isMoving;
    float lerpTime;

    public void PlaceMemberWord(Component sender, object obj)
    {
        GameObject memberWord = (GameObject)obj;

        Transform pivot = GetEmptyPlace();

        if (pivot == null)
            return;

        memberWord.transform.SetParent(pivot);

        if (MoveMemberWordToHandSequence != null && MoveMemberWordToHandSequence.IsActive())
            MoveMemberWordToHandSequence.Kill();

        currentMemberWord = memberWord;
        currentTarget = pivot;

        isMoving = true;
        lerpTime = 0;

        MoveMemberWordToHandSequence = DOTween.Sequence();

        MoveMemberWordToHandSequence
            .Append(
                DOTween.To(
                    () => lerpTime,
                    x => lerpTime = x,
                    1,
                    MoveDuration
                ).SetEase(Ease.InOutSine)
            )
            .OnComplete(() =>
            {
                isMoving = false;

                if (currentMemberWord != null && currentTarget != null)
                {
                    currentMemberWord.transform.position = currentTarget.position;
                    currentMemberWord.transform.rotation = currentTarget.rotation;
                }
            });
    }

    private void Update()
    {
        if (!isMoving || currentMemberWord == null || currentTarget == null)
            return;

        currentMemberWord.transform.position = Vector3.Lerp(
            currentMemberWord.transform.position,
            currentTarget.position,
            lerpTime
        );

        currentMemberWord.transform.rotation = Quaternion.Lerp(
            currentMemberWord.transform.rotation,
            currentTarget.rotation,
            lerpTime
        );
    }

    private Transform GetEmptyPlace()
    {
        foreach (Transform pivot in Pivots)
        {
            if (pivot.childCount == 0)
                return pivot;
        }

        return null;
    }



}
