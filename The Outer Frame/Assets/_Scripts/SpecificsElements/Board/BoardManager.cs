using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] GameEvent OnPlacedNewBoardInformation;
    [SerializeField] GameEvent OnBoardPlacedPhotos;
    [SerializeField] GameEvent OnBoardPlacedConections;
    [SerializeField] GameEvent OnRefreshInfoInBoard;
    [SerializeField] Transform StartPos;
    bool IsInView;


    public void StateView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.BoardView)
        {
            IsInView = true;
            OnPlacedNewBoardInformation?.Invoke(null, StartPos.position);
            OnRefreshInfoInBoard?.Invoke(this, null);
        }
        else IsInView = false;
    }

    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (!IsInView) return;
        OnBoardPlacedPhotos?.Invoke(null, StartPos.position);
        Invoke("Conections", 0.5f);
    }

    void Conections()
    {
        OnBoardPlacedConections?.Invoke(this, null);
    }
}
