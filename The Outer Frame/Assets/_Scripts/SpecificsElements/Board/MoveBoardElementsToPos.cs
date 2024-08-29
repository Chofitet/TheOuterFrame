using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveBoardElementsToPos : MonoBehaviour
{
    IPlacedOnBoard conditions;
    Vector3 FinalPosition;
    Vector3 FinalRotation;
    GameObject Content;
    bool isPlaced;
    bool isTaken;
    bool toReplece;
    private void Start()
    {
        FinalPosition = transform.position;
        FinalRotation = transform.rotation.eulerAngles;
        conditions = GetComponent<IPlacedOnBoard>();
        Content = transform.GetChild(0).gameObject;
        if (!conditions.ActiveInBegining())
        {
            Content.SetActive(false);
        }
    }

    public void SetToReplace() => toReplece = true;

    public void MoveToPlacedPos(Component sender, object obj)
    {
        if (isPlaced) return;
        if (!conditions.GetConditionalState() && !toReplece) return;

        Vector3 InitPos = (Vector3)obj;
        
        
        if (isTaken)
        {
            StartCoroutine(Delay(InitPos));
            isTaken = false;
            return;
        }
        isPlaced = true;
        toReplece = false;
        StartCoroutine(Delay(InitPos));
    } 

    IEnumerator Delay(Vector3 InitPos)
    {
        yield return new WaitForSeconds(0f);
        transform.position = InitPos;

        Content.SetActive(true);

        transform.DOMove(FinalPosition, 1f).SetEase(Ease.InOutQuad);
        transform.DORotate(FinalRotation, 0.3f).SetEase(Ease.InOutCirc);
    }

    public bool  GetIsPlaced()
    {
        return isPlaced;
    }
}
