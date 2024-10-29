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
    public void moveObjectToThisPos(Component sender, object obj)
    {
        if (LastObj) return;
        GameObject _object = (GameObject)obj;
        LastObj = _object;
        initPos = LastObj.transform.position;
        initRot = LastObj.transform.rotation.eulerAngles;
        tweenPos = LastObj.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InOutCirc);
        tweenRot = LastObj.transform.DORotate(transform.rotation.eulerAngles, 0.3f).SetEase(Ease.InOutCirc);
    }
    public void BackLastObjectToPos(Component sender, object obj)
    {
        float offset = 0;
        if (sender is DossierMoveController) offset = 0.03f;
        if (!LastObj) return;
        tweenPos.Kill();
        tweenRot.Kill();
        tweenPos = LastObj.transform.DOMove(LastObj.transform.position + new Vector3(offset, 0, 0), 0.2f)
        .SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            tweenPos = LastObj.transform.DOMove(initPos, 0.5f)
                .SetEase(Ease.InOutSine);
            tweenRot = LastObj.transform.DORotate(initRot, 0.3f).SetEase(Ease.InOutSine).OnComplete(() => { LastObj = null; });
        });
        
        
    }

    public void DeleteLastObject(Component sender, object obj)
    {
        LastObj = null;
    }
}
