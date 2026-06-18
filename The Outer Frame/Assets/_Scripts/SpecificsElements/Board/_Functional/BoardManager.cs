using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] List<GeneratorActionController> IdeasToCheckConditionals = new List<GeneratorActionController>();
    [SerializeField] GameEvent OnSendIdeaConditionalBool;
    [SerializeField] MoveBoardElementsToPos[] SetInTutorial;
    [SerializeField] StringConnectionController[] ConnectInTutorial;
    [SerializeField] GameEvent OnEnableInput;
    [SerializeField] GameEvent OnDisableInput;
    [SerializeField] GameEvent OnConnectStringByClicking;

    [Header("CallsToUpdate")]
    [SerializeField] GameEvent OnInactiveIdeas;
    [SerializeField] GameEvent OnUpdatePhotoUpdate;
    [SerializeField] GameEvent OnUpdateConnections;
    [SerializeField] GameEvent OnUpdatePosits;
    [SerializeField] GameEvent OnUpdateIdeas;
    

    int WordsCounts;
    bool IsInView;
    bool isInTutorial;
    Coroutine updateCorrutine;
    public void StateView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        CheckIdeaConditionals();

        if (view == ViewStates.BoardView)
        {
            IsInView = true;
            if (updateCorrutine != null) StopCoroutine(updateCorrutine);
            updateCorrutine = StartCoroutine(UpdateCycle(0.5f));
            OnRefreshInfoInBoard?.Invoke(this, null);
            
        }
        else IsInView = false;
    }

    public void OnSelectWordInNotebook(Component sender, object obj)
    {
        if (!IsInView) return;
        OnBoardPlacedPhotos?.Invoke(null, StartPos.position);
        OnAutoUpdatePreviusPhoto?.Invoke(null, StartPos.position);
        //Invoke("Conections", 0.6f);
        if(updateCorrutine != null) StopCoroutine(updateCorrutine);
        updateCorrutine = StartCoroutine(UpdateCycle(0));
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
       // StartCoroutine(WaitUpdating());
    }

    IEnumerator WaitUpdating()
    {
        OnDisableInput.Invoke(this, null);
        yield return new WaitForSeconds(0.5f);
        OnEnableInput?.Invoke(this, null);
    }

    IEnumerator UpdateCycle(float waitTime)
    {
        int cycles = 0;

        
        yield return new WaitForSeconds(waitTime);
       
        while (cycles < 3)
        {
            yield return StartCoroutine(UpdateElements());
            OnRefreshNotebook?.Invoke(this, null);
            cycles++;
        }

        OnEnableInput.Invoke(this, null);
        
        WaitPhotoUpdate = 0;
        WaitPosit = 0;
        WaitIdeas = 0;
    }

    IEnumerator UpdateElements()
    {
        if (!IsInView) yield break;
        OnDisableInput.Invoke(this, null);
        OnInactiveIdeas?.Invoke(this, null);
        OnUpdatePhotoUpdate?.Invoke(this, StartPos.position);
        yield return new WaitForSeconds(WaitPhotoUpdate);
        OnUpdatePosits.Invoke(this, StartPos.position);
        yield return new WaitForSeconds(WaitPosit);
        OnUpdateIdeas.Invoke(this, StartPos.position);
        yield return new WaitForSeconds(WaitIdeas);
        //OnTakeOutInfoInBoard?.Invoke(this, TakeOutPos);
        yield return new WaitForSeconds(WaitConnections);
        OnUpdateConnections.Invoke(this, null);

    }

    float WaitPhotoUpdate = 0f;
    float WaitPosit = 0f;
    float WaitIdeas = 0f;
    float WaitConnections = 0f;

    public void ActualTypeOfElementMoving(Component sender, object obj)
    {
        BoardType typeOf = (BoardType) obj;

        WaitPhotoUpdate = 0;
        WaitPosit = 0;
        WaitIdeas = 0;
        WaitConnections = 0;

        if (typeOf == BoardType.photoUpdate)
        {
            WaitPhotoUpdate = 0.3f;
        }
        else if(typeOf == BoardType.posit)
        {
            WaitPosit = 0.3f;
        }
        else if(typeOf == BoardType.posit)
        {
            WaitIdeas = 0.3f;
        }
        else if(typeOf == BoardType.connection)
        {
            WaitConnections = 0.3f;
        }
    }

    public void MadeConnections(Component sender, object obj)
    {
        OnConnectStringByClicking?.Invoke(this, null);
    }

    void CheckIdeaConditionals()
    {
        foreach(GeneratorActionController idea in IdeasToCheckConditionals)
        {
            bool conditional = idea.GetComponent<IPlacedOnBoard>().GetConditionalState();
            if (conditional) OnSendIdeaConditionalBool?.Invoke(this, null);
        }
    }
}
