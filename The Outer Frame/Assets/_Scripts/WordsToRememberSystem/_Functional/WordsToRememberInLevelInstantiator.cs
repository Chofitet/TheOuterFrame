using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordsToRememberInLevelInstantiator : MonoBehaviour
{
    [SerializeField] GameObject MemberWordPrefab;
    [SerializeField] List<Transform> anchors;
    [SerializeField] Transform Anchor;
    [SerializeField] Transform FinalAnchorPosition;
    [SerializeField] float TimeToDownMemberWords;
    [SerializeField] GameEvent OnSetStartView;
 

    public void InstanciateMemberWords(Component sender, object obj)
    {
        List<WordData> WordsToRemember = (List<WordData>)obj;

        int index = 0;

        foreach (WordData memberWord in WordsToRemember)
        {
            Transform actualAnchor = anchors[index];

            GameObject instantiateWord =  Instantiate(MemberWordPrefab, actualAnchor.position, actualAnchor.rotation);
            instantiateWord.GetComponent<WordToRememberInLevel>().Initialize(memberWord);
            instantiateWord.transform.SetParent(actualAnchor);
            index++;
        }

        if (WordsToRemember.Count > 0) OnSetStartView?.Invoke(this, new StartViewData(ViewStates.DossierView,1.2f, TimeToDownMemberWords));

    }

}
