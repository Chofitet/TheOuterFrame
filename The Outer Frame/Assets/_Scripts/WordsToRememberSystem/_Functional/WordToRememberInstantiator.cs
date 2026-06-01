using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordToRememberInstantiator : MonoBehaviour
{
    [SerializeField] GameObject WordToRememberPrefab;
    [SerializeField] GameObject Anchors;
    int WordsToRememberAmount;
    [SerializeField] GameEvent OnChangeScene;
    [SerializeField] List<WordData> DebugMemberWords;
    [SerializeField] List<Transform> Pivots;
    bool isInBackView;
    public void ShowRememberWordsInVoid(Component sender,object obj)
    { 
        List<WordData> WordsToRemember = (List<WordData>)obj;

        if (DebugMemberWords.Count != 0) WordsToRemember = DebugMemberWords;

        WordsToRememberAmount = WordsToRemember.Count;

        Transform[] anchorTransforms = Anchors.GetComponentsInChildren<Transform>();

        for (int i = 0; i < WordsToRemember.Count; i++)
        {
            Transform anchor = anchorTransforms[i + 1];

            GameObject wordToRemember = Instantiate(
                WordToRememberPrefab,
                anchor.position,
                anchor.rotation,
                anchor
            );

            wordToRemember.GetComponent<WordToRemember>()
                .Initialize(WordsToRemember[i]);
        }
    }

    int MemberWordsTaked;
    public void CheckMembersWordsTaked(Component sender,object obj)
    {
        MemberWordsTaked += 1;

        if (WordsToRememberAmount == MemberWordsTaked)
        {
            OnChangeScene?.Invoke(this, null);
        }
    }

    public void UncheckMembersWordsTaked(Component sender, object obj)
    {
        MemberWordsTaked -= 1;
    }

    Sequence MoveMemberWordToSpaceSequence;

    public void PlaceMemberWord(Component sender, object obj)
    {
        GameObject memberWord = (GameObject)obj;

        Transform pivot = GetEmptyPlace();

        if (MoveMemberWordToSpaceSequence != null & MoveMemberWordToSpaceSequence.IsActive()) MoveMemberWordToSpaceSequence.Kill();

        MoveMemberWordToSpaceSequence = DOTween.Sequence();

        if (isInBackView)
        {
            MoveMemberWordToSpaceSequence.PrependInterval(0.5f)
                .AppendCallback(()=>
                {
                    memberWord.transform.SetParent(pivot);
                })
            .Append(memberWord.transform.DOMove(pivot.position, 0.3f))
              .Join(memberWord.transform.DORotate(pivot.rotation.eulerAngles, 0.3f));
        }
        else
        {
            memberWord.transform.SetParent(pivot);
            MoveMemberWordToSpaceSequence.Append(memberWord.transform.DOMove(pivot.position, 0.3f))
              .Join(memberWord.transform.DORotate(pivot.rotation.eulerAngles, 0.3f));
        }

        
        
    }

    Transform GetEmptyPlace()
    {
        foreach (Transform pivot in Pivots)
        {
            if (pivot.childCount == 0) return pivot;
        }

        return null;
    }

    public void IsInBackView(Component sender, object obj)
    {
        isInBackView = true;
    }

    public void IsInDefaultView(Component sender, object obj)
    {
        isInBackView = false;
    }
}
