using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Profiling;
using TMPro.Examples;

public class WordToRemember : MonoBehaviour
{
    WordData word;
    [SerializeField] List<TMP_Text> textFields;
    [SerializeField] GameEvent OnAddMemberWord;
    [SerializeField] GameEvent OnRemoveMemberWord;
    [SerializeField] GameEvent OnBackToDefaultPosInVoid;
    [SerializeField] BoxCollider blockInput;
    [SerializeField] List<MemberWordsModels> models = new List<MemberWordsModels>();
    [SerializeField] List<GameObject> PapersGOs = new List<GameObject>();
    [SerializeField] List<GameEvent> OnGrabPapersSounds = new List<GameEvent>();
    [SerializeField] List<GameEvent> OnBackPapersSounds = new List<GameEvent>();
    Transform takePosition;
    Transform idlePosition;
    float takeDuration;
    float leaveDuration;
    bool isTaken;
    int ChosenPaper;
    static readonly ProfilerMarker WarpMarker = new ProfilerMarker("WarpText Update");
    public void Initialize(WordData _word, List<int> ChosenPapersList, float _takeDuration, Transform _takePosition, Transform _IdlePosition, float _leaveDuration)
    {
        word = _word;
        foreach (TMP_Text textField in textFields)
        {
            textField.text = _word.GetName();

            Invoke("UpdateMesh", 0.1f);
        }
        foreach (GameObject paper in PapersGOs) paper.SetActive(false);

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

    void UpdateMesh()
    {
        foreach (TMP_Text textField in textFields)
        {
            if (textField.IsActive()) textField.GetComponent<WarpTextExample>().UpdateText();
        }
    }
    public int GetChosenPaper() { return ChosenPaper; }

    public void AddWordToMemberList(Component sender, object obj)
    {
        if ((GameObject)obj != gameObject) return;

        WarpMarker.Begin();

        if (!isTaken)
        {
            OnAddMemberWord?.Invoke(this, gameObject);
            isTaken = true;
            StartCoroutine(BlockInput(0.3f));
            MoveToHand(takePosition, takeDuration);

            OnGrabPapersSounds[int.Parse(idlePosition.name)]?.Invoke(this,null);
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
        WarpMarker.End();
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
        transform.SetParent(target);
        foreach (TMP_Text textField in textFields) if (textField.IsActive()) textField.GetComponent<WarpTextExample>().UpdateText();

            moveSequence = DOTween.Sequence();
        isFollowingTarget = true;
        lerpTime = 0;

        moveSequence.Append(
                DOTween.To(() => lerpTime,x => lerpTime = x,1f,duration)
            )
            .OnComplete(() =>
            {
                isFollowingTarget = false;
                
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
        .JoinCallback(()=> OnBackPapersSounds[int.Parse(idlePosition.name)]?.Invoke(this, null))
        .OnComplete(() =>
        {
            isFollowingTarget = false;
            currentTarget = null;
            transform.SetParent(pivot);
            foreach (TMP_Text textField in textFields) if(textField.IsActive()) textField.GetComponent<WarpTextExample>().UpdateText();
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