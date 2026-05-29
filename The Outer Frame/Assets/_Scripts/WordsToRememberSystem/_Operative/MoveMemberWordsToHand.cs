using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MoveMemberWordsToHand : MonoBehaviour
{
    [SerializeField] List<Transform> Pivots = new List<Transform>();
    Sequence MoveMemberWordToHandSequence;


    public void PlaceMemberWord(Component sender, object obj)
    {
        GameObject memberWord = (GameObject)obj;

        Transform pivot = GetEmptyPlace();

        memberWord.transform.SetParent(pivot);

        //if(MoveMemberWordToHandSequence != null && MoveMemberWordToHandSequence.IsActive()) MoveMemberWordToHandSequence.Kill();

        MoveMemberWordToHandSequence.Append(memberWord.transform.DOMove(pivot.position, 0.3f))
              .Join(memberWord.transform.DORotate(pivot.rotation.eulerAngles, 0.3f));
    }

    Transform GetEmptyPlace()
    {
        foreach(Transform pivot in Pivots)
        {
            if (pivot.childCount == 0) return pivot;
        }

        return null;
    }

}
