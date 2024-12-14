using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveObjectToThisPos : MonoBehaviour
{
    Vector3 initPos;
    Vector3 initRot;
    GameObject LastObj;
    Tween tweenPos;
    Tween tweenRot;
    Sequence MoveSequence;
    GameObject newObject;

    public void moveObjectToThisPos(Component sender, object obj)
    {
        if (LastObj)
        {
            GameObject anotherObject = (GameObject)obj;
            if (anotherObject != LastObj)
            {
                BackLastObjectToPos(null, LastObj);
                MoveObject(anotherObject, 0.2f);
            }
            return;
        }
        GameObject _object = (GameObject)obj;

        MoveObject(_object, 0);
    }

    void MoveObject(GameObject _object, float TimeToWait)
    {
        LastObj = _object;
        initPos = LastObj.transform.position;
        initRot = LastObj.transform.rotation.eulerAngles;

        if (MoveSequence != null && MoveSequence.IsActive()) MoveSequence.Kill();

        MoveSequence = DOTween.Sequence();

        MoveSequence.AppendInterval(TimeToWait)
                    .Append(LastObj.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InOutCirc))
                    .Join(LastObj.transform.DORotate(transform.rotation.eulerAngles, 0.3f).SetEase(Ease.InOutCirc));
    }
    public void BackLastObjectToPos(Component sender, object obj)
    {
        if (!LastObj) return;

        GameObject objectToBack;
        bool isReplaced = false;

        if (obj == null) objectToBack = LastObj;
        else
        {
            objectToBack = (GameObject)obj;
            isReplaced = true;
        }

        if (MoveSequence != null && MoveSequence.IsActive()) MoveSequence.Kill();

        Sequence BackSequence = DOTween.Sequence();

        BackSequence.Append(objectToBack.transform.DOMove(initPos, 0.5f).SetEase(Ease.InOutCirc))
                    .Join(objectToBack.transform.DORotate(initRot, 0.3f).SetEase(Ease.InOutCirc))
                    .OnComplete(()=> 
                    { 
                        if(!isReplaced) LastObj = null;
                        objectToBack = null;
                    });


    }


    public void DeleteLastObject(Component sender, object obj)
    {
        LastObj = null;
    }
}
