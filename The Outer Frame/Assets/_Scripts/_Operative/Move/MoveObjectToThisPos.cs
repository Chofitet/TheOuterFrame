using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MoveObjectToThisPos : MonoBehaviour
{
    Vector3 initPos;
    Vector3 initRot;
    GameObject LastObj;
    Tween tweenPos;
    Tween tweenRot;
    Sequence MoveSequence;
    GameObject newObject;
    Transform CurrentTarget;

    [SerializeField] List<BoardElementsTransforms> Positions = new List<BoardElementsTransforms>();


    public void moveObjectToThisPos(Component sender, object obj)
    {
        if (LastObj)
        {
            GameObject anotherObject = (GameObject)obj;

            ListOfBoardElementsPositions _elementPosition = anotherObject.GetComponent<ListOfBoardElementsPositions>();
            if(_elementPosition != null ) CurrentTarget = SearchElementPosition(_elementPosition.BoardElementsPosition);
            else  CurrentTarget = transform;

            if (anotherObject != LastObj)
            {
                BackLastObjectToPos(null, LastObj);
                MoveObject(anotherObject, 0.2f);
            }
            else
            {
                MoveObject(anotherObject, 0);
            }
            return;
        }

        GameObject _object = (GameObject)obj;

        ListOfBoardElementsPositions elementPosition = _object.GetComponent<ListOfBoardElementsPositions>();
        if (elementPosition != null) CurrentTarget = SearchElementPosition(elementPosition.BoardElementsPosition);
        else CurrentTarget = transform;

        MoveObject(_object, 0);
    }

    bool inMovingToPosition;

    void MoveObject(GameObject _object, float TimeToWait)
    {
        inMovingToPosition = true;
        LastObj = _object;
        initPos = LastObj.transform.position;
        initRot = LastObj.transform.rotation.eulerAngles;
        LastObj.GetComponent<BoxCollider>().enabled = false;

        if (MoveSequence != null && MoveSequence.IsActive()) MoveSequence.Kill();

        MoveSequence = DOTween.Sequence();

        MoveSequence.AppendInterval(TimeToWait)
                    .Append(LastObj.transform.DOMove(CurrentTarget.position, 0.5f).SetEase(Ease.InOutCirc))
                    .Join(LastObj.transform.DORotate(CurrentTarget.rotation.eulerAngles, 0.3f).SetEase(Ease.InOutCirc))
                    .OnComplete(() =>
                    {
                        inMovingToPosition = false;
                    });
    }
    public void BackLastObjectToPos(Component sender, object obj)
    {
        if (!LastObj)
        {
           // Debug.Log("Any positsToReturn");
            return;
        }


        GameObject objectToBack;
        bool isReplaced = false;

        if (obj == null || obj is not GameObject) objectToBack = LastObj;
        else
        {
            objectToBack = (GameObject)obj;
            isReplaced = true;
        }

        if (MoveSequence != null && MoveSequence.IsActive()) MoveSequence.Kill();

        if (objectToBack == null) return;

        Sequence BackSequence = DOTween.Sequence();

        BackSequence.Append(objectToBack.transform.DOMove(initPos, 0.5f).SetEase(Ease.InOutCirc))
                    .Join(objectToBack.transform.DORotate(initRot, 0.3f).SetEase(Ease.InOutCirc))
                    .OnComplete(()=> 
                    {
                        LastObj.GetComponent<BoxCollider>().enabled = false;
                        objectToBack.GetComponent<BoxCollider>().enabled = true;
                        if (!isReplaced && !inMovingToPosition) LastObj = null;
                        objectToBack = null;
                        
                    });


    }


    public void DeleteLastObject(Component sender, object obj)
    {
        LastObj = null;
    }

    Transform SearchElementPosition(BoardElementsPositions indexPos)
    {
        foreach(BoardElementsTransforms pos in Positions)
        {
            if(pos.GetPosition(indexPos) != null)
            {
                return pos.GetPosition(indexPos);
            }
        }
        return null;
    }
}

[Serializable]
public class BoardElementsTransforms
{
    public BoardElementsPositions position;
    public Transform transform;

    public Transform GetPosition(BoardElementsPositions indexPos)
    {
        if (indexPos == position) return transform;

        return null;
    }
}