using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] GameEvent OnPlacedNewBoardInformation;
    [SerializeField] GameEvent OnBoardPlacedPhotos;
    [SerializeField] GameEvent OnBoardPlacedConections;
    [SerializeField] GameEvent OnRefreshInfoInBoard;
    [SerializeField] GameEvent OnTakeOutInfoInBoard;
    [SerializeField] GameEvent OnRefreshNotebook;
    [SerializeField] GameEvent OnAutoUpdatePreviusPhoto;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform TakeOutPos;
    [SerializeField] GameEvent OnPlaced4WordsInBoard;
    [SerializeField] MoveBoardElementsToPos[] SetInTutorial;
    [SerializeField] StringConnectionController[] ConnectInTutorial;
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDisableInput;
    int WordsCounts;
    bool IsInView;
    bool isInTutorial;
    bool isInUpdatingTime;


    public void StateView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.BoardView)
        {
            StartCoroutine(TurnOffisInUpdatingTime());
            IsInView = true;
            OnPlacedNewBoardInformation?.Invoke(null, StartPos.position);
            OnRefreshInfoInBoard?.Invoke(this, null);
            OnTakeOutInfoInBoard?.Invoke(this, TakeOutPos);
        }
        else IsInView = false;
    }

    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (!IsInView) return;
        OnBoardPlacedPhotos?.Invoke(null, StartPos.position);
        OnAutoUpdatePreviusPhoto?.Invoke(null, StartPos.position);
        Invoke("Conections", 0.6f);
        WordsCounts += 1;
        if (WordsCounts == 4 && isInTutorial) OnPlaced4WordsInBoard?.Invoke(this, null);
    }

    void Conections()
    {
        OnBoardPlacedConections?.Invoke(this, null);
        StartCoroutine(RefreshInfo());
    }


    IEnumerator RefreshInfo()
    {
        yield return new WaitForSeconds(1f);
        OnPlacedNewBoardInformation?.Invoke(null, StartPos.position);
        OnRefreshInfoInBoard?.Invoke(this, null);
        OnRefreshNotebook?.Invoke(this, null);
        
    }

    public void SetIsInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;

        if (!isInTutorial)
        {
            Invoke("ActiveElements", 0.2f);
            Invoke("MakeConections", 0.2f);
        }
    }

    void ActiveElements()
    {
        foreach (MoveBoardElementsToPos BoardElement in SetInTutorial)
        {
            if (!BoardElement) continue;
            BoardElement.PlaceDirectly();
        }
    }

    void MakeConections()
    {
        foreach (StringConnectionController BoardElement in ConnectInTutorial)
        {
            if (!BoardElement) continue;
            BoardElement.ConnectDirectly();
        }
    }

    public void UpdatingSomethingInBoard(Component sender, object obj)
    {
        //if (!isInUpdatingTime) return;
        StartCoroutine(WaitUpdating());
    }

    IEnumerator WaitUpdating()
    {
        OnDisableInput.Invoke(this, null);
        yield return new WaitForSeconds(0.5f);
        OnEnableInput?.Invoke(this, null);
    }

    IEnumerator TurnOffisInUpdatingTime()
    {
        isInUpdatingTime = true;
        yield return new WaitForSeconds(0.7f);
        isInUpdatingTime = false;
    }
}
