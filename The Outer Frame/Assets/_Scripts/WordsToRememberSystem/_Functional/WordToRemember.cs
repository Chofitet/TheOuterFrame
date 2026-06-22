using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordToRemember : MonoBehaviour
{
    WordData word;
    [SerializeField] TMP_Text textField;
    [SerializeField] GameEvent OnAddMemberWord;
    [SerializeField] GameEvent OnRemoveMemberWord;
    [SerializeField] GameEvent OnBackToDefaultPosInVoid;
    [SerializeField] BoxCollider blockInput;
    [SerializeField] List<MemberWordsModels> models = new List<MemberWordsModels>();
    Transform takePosition;
    Transform idlePosition;
    float takeDuration;
    float leaveDuration;
    bool isTaken;
    int ChosenPaper;

    public void Initialize(WordData _word, List<int> ChosenPapersList, float _takeDuration, Transform _takePosition, Transform _IdlePosition, float _leaveDuration)
    {
        word = _word;
        textField.text = _word.GetName();
        takeDuration = _takeDuration;
        takePosition = _takePosition;
        idlePosition = _IdlePosition;
        leaveDuration = _leaveDuration;
        int index = 0;
        foreach (MemberWordsModels paperModelData in models)
        {


            GameObject paper = paperModelData.GetModel(word.GetName().Length);


            if (paper)
            {
                ChosenPaper = index;
                index++;

                if (ChosenPapersList.Contains(ChosenPaper)) continue;
                paper.SetActive(true);

                word.SetMemberWordNumPaperModel(ChosenPaper);
                return;
            }
            else index++;

        }
    }

    public int GetChosenPaper() { return ChosenPaper; }

    public void AddWordToMemberList(Component sender, object obj)
    {
        if ((GameObject)obj != gameObject) return;



        if (!isTaken)
        {
            OnAddMemberWord?.Invoke(this, gameObject);
            isTaken = true;
            StartCoroutine(BlockInput(0.3f));
            MoveToHand(takePosition, takeDuration);
        }
        else
        {
            OnBackToDefaultPosInVoid?.Invoke(this, null);
            OnRemoveMemberWord?.Invoke(this, gameObject);
            isTaken = false;
            MoveToPivot(idlePosition, leaveDuration, isInBackView);
            if (isInBackView) StartCoroutine(BlockInput(1.3f));
            else StartCoroutine(BlockInput(0.3f));
        }
    }

    IEnumerator BlockInput(float time)
    {
        blockInput.enabled = true;
        yield return new WaitForSeconds(time);
        blockInput.enabled = false;
    }

    bool isInBackView;
    public void IsInBackView(Component sender, object obj)
    {
        isInBackView = true;
    }

    public void IsInDefaultView(Component sender, object obj)
    {
       if(isInBackView)Invoke("SetBackViewFalse", 0.5f);
    }

    void SetBackViewFalse()
    {
        isInBackView = false;
    }

    public void SetWordModel(int CharactersNum)
    {

    }

    public WordData GetWord() { return word; }


    private Sequence moveSequence;
    private Transform currentTarget;
    private float lerpTime;
    private bool isFollowingTarget;

    private void Update()
    {
        if (!isFollowingTarget || currentTarget == null)
            return;

        transform.position = Vector3.Lerp(transform.position, currentTarget.position,lerpTime);

        transform.rotation = Quaternion.Lerp(transform.rotation,currentTarget.rotation,lerpTime);
    }

    public void MoveToHand(Transform target, float duration)
    {
        if (moveSequence != null && moveSequence.IsActive()) moveSequence.Kill();

        currentTarget = target;
        

        moveSequence = DOTween.Sequence();
        isFollowingTarget = true;
        lerpTime = 0;

        moveSequence.Append(
                DOTween.To(() => lerpTime,x => lerpTime = x,1f,duration)
            )
            .OnComplete(() =>
            {
                isFollowingTarget = false;
                transform.SetParent(target);
            });
    }

    public void MoveToPivot(Transform pivot, float duration, bool isInBackView)
    {
        if (moveSequence != null && moveSequence.IsActive())
            moveSequence.Kill();

        moveSequence = DOTween.Sequence();
        currentTarget = pivot;
        lerpTime = 0f;
        isFollowingTarget = true;

        if (isInBackView)
        {
            moveSequence
                .PrependInterval(0.7f);
        }

        moveSequence
        .Append(
            DOTween.To(  () => lerpTime, x => lerpTime = x, 1f, duration)
        )
        .OnComplete(() =>
        {
            isFollowingTarget = false;
            currentTarget = null;
            transform.SetParent(pivot);
        });

    }
}

[Serializable]
public class MemberWordsModels
{
    public GameObject model;
    public int MaxWordLenght;

    public GameObject GetModel(int NumCharacters)
    {
        if(NumCharacters < MaxWordLenght) return model;

        return null;
    }
}