using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustBoardCollider : MonoBehaviour
{
    BoxCollider _collider;
    Vector3 InitialPos;
    [SerializeField] BoxCollider GeneralButton;
    [SerializeField] BoxCollider TakeSomeInBoardButton;

    public void AdjustCollider(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;


        if(view == ViewStates.OnTakeSomeInBoard)
        {
            TakeSomeInBoardButton.enabled = true;
            GeneralButton.enabled = false;
        }
        else if (view == ViewStates.BoardView) 
        {
            TakeSomeInBoardButton.enabled = false;
            GeneralButton.enabled = false;
        }
        else
        {
            TakeSomeInBoardButton.enabled = false;
            GeneralButton.enabled = true;
        }
    }

   
}
