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
    bool IsInView;


    public void StateView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.BoardView)
        {
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
        OnRefreshNotebook?.Invoke(this,null);
    }
}
