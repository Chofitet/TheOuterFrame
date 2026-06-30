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
    [SerializeField] List<Transform> HandPivots;
    List<int> ChosenPapers = new List<int>();
    [SerializeField] float AnimTakeWordTime = 0.3f;
    [SerializeField] float AnimLeaveWordTime;
    [SerializeField] GameEvent OnTakeDownMemberPapers;

    [SerializeField] float TimeToChangeScene;
    bool CallOnce;
    public void ShowRememberWordsInVoid(Component sender,object obj)
    {
        if (CallOnce) return;
        List<WordData> WordsToRemember = (List<WordData>)obj;

        if (DebugMemberWords.Count != 0) WordsToRemember = DebugMemberWords;

        WordsToRememberAmount = WordsToRemember.Count;

        Transform[] anchorTransforms = Anchors.GetComponentsInChildren<Transform>();

        for (int i = 0; i < WordsToRemember.Count; i++)
        {
            Transform anchor = anchorTransforms[i + 1];

            GameObject wordToRemember = Instantiate( WordToRememberPrefab, anchor.position,anchor.rotation, anchor);

            wordToRemember.GetComponent<WordToRemember>()
                .Initialize(WordsToRemember[i], ChosenPapers, AnimTakeWordTime, HandPivots[i], anchor, AnimLeaveWordTime);

            ChosenPapers.Add(wordToRemember.GetComponent<WordToRemember>().GetChosenPaper());
        }

        CallOnce = true;
    }

    int MemberWordsTaked;
    public void CheckMembersWordsTaked(Component sender,object obj)
    {
        MemberWordsTaked += 1;

        if (WordsToRememberAmount == MemberWordsTaked)
        {
            Invoke("changeScene", TimeToChangeScene);
            OnTakeDownMemberPapers?.Invoke(this, MemberWordsTaked);
        }
    }

    public void CheckOnExitBackVoid(Component sender,object obj)
    {
        Invoke("changeScene", TimeToChangeScene);
        OnTakeDownMemberPapers?.Invoke(this, MemberWordsTaked);
    }

    void changeScene()
    {
        OnChangeScene?.Invoke(this, "LoadingScreen");
    }

    public void UncheckMembersWordsTaked(Component sender, object obj)
    {
        MemberWordsTaked -= 1;
    }

}
