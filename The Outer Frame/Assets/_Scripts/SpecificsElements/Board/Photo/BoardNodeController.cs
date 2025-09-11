using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardNodeController : MonoBehaviour, IPlacedOnBoard
{
    [SerializeField] WordData word;

    [SerializeField] GameObject PostItConteiner;
    [SerializeField] GameEvent OnPutPhotoOnBoard;
    MoveBoardElementsToPos[] PostIts;
    [SerializeField] GameObject PhotoModel;
    [SerializeField] GameObject Canvas;
    bool AwaitingForPlace;

    private void Start()
    {
        if (!word) Debug.LogWarning("Board Node " + name + " dont have any word assigned");

        PostIts = PostItConteiner.GetComponentsInChildren<MoveBoardElementsToPos>();
    }

    public bool GetConditionalState()
    {
        if (word.GetIsFound() && WordSelectedInNotebook.Notebook.GetSelectedWord() == word || CheckRetroactiveWordWasPlaced() || (!AwaitingForPlace && word.GetPlacedInBoard()))
        {
            transform.position = new Vector3(0, 0, 0);
            transform.GetChild(0).gameObject.SetActive(true);
            ActiveChildPosits();
            MarkRetroactiveWordPlaced();
            OnPutPhotoOnBoard?.Invoke(this, word);
            word.SetPlacedInBoard();
            AwaitingForPlace = true;
            //ActiveOtherPhotoReplaced();
            return true;
        }
        else return false;
    }

        void ActiveChildPosits()
    {
        foreach(MoveBoardElementsToPos posit in PostIts)
        {
            if (posit.GetComponent<IPlacedOnBoard>().GetIsTaken()) return;
            posit.MoveToPlacedPos(null, transform.position);

            if (posit.GetIsAUpdatedPhoto() && posit.GetIsPlaced())
            {
                PhotoModel.SetActive(false);
                Canvas.SetActive(false);
            }
        }
    }

    bool CheckRetroactiveWordWasPlaced() 
    {
        if (word.SearchForWordsThatReplaceRetroactive().Count == 0) return false;

        foreach(WordData previousWord in word.SearchForWordsThatReplaceRetroactive())
        {
            if (previousWord.GetPlacedInBoard() && word.GetIsFound()) return true;
        }

        return false;
    }

    void MarkRetroactiveWordPlaced()
    {
        foreach (WordData previousWord in word.SearchForWordsThatReplaceRetroactive())
        {
            previousWord.SetPlacedInBoard();
        }
    }

   /* void ActiveOtherPhotoReplaced()
    {
        if (!WordReplace) return;
        if (WordReplace.GetIsPlaced()) return;
        WordReplace.SetToReplace();
        WordReplace.MoveToPlacedPos(null, transform.position);
    }*/

    public bool ActiveInBegining()
    {
       return false;
    }

    public bool GetIsTaken()
    {
        return false;
    }

    public bool IsOutOfBoard()
    {
        return false;
    }

}
