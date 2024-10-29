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

    private void Start()
    {
        if (!word) Debug.LogWarning("Board Node " + name + " dont have any word assigned");

        PostIts = PostItConteiner.GetComponentsInChildren<MoveBoardElementsToPos>();
    }

    public bool GetConditionalState()
    {
        if (word.GetIsFound() && WordSelectedInNotebook.Notebook.GetSelectedWord() == word)
        {
            transform.position = new Vector3(0, 0, 0);
            transform.GetChild(0).gameObject.SetActive(true);
            ActiveChildPosits();
            OnPutPhotoOnBoard?.Invoke(this, word);
            word.SetPlacedInBoard();
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
