using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MoveBoardElementsToPos : MonoBehaviour
{
    IPlacedOnBoard conditions = null;
    [SerializeField] GameEvent OnPlaceInBoardSound;
    [SerializeField] bool IsAUpdatedPhoto;
    Vector3 FinalPosition;
    Vector3 FinalRotation;
    GameObject Content;
    bool isPlaced;
    bool isTaken;
    bool toReplece;
    bool isOutOfBoard;
    private void Start()
    {
        UpdateFinalPositionRotation(null, null);
        conditions = GetComponent<IPlacedOnBoard>();

        if (conditions == null)
        {
            Debug.Log(transform.parent.parent.parent.gameObject.name);
        }

        if(name == "QuiteClean")
        {
            Debug.Log(transform.parent.parent.parent.gameObject.name);

        }
        Content = transform.GetChild(0).gameObject;
        if (!conditions.ActiveInBegining())
        {
            Invoke("sarasa", 0.1f);
        }
    }


    public void UpdateFinalPositionRotation(Component sender, object obj)
    {
        if(Content != null)
        {
            // Buscar TODOS los hijos, incluso los inactivos
            var childrenWithMover = Content.GetComponentsInChildren<MoveBoardElementsToPos>(true);

            foreach (var mover in childrenWithMover)
            {
                GameObject go = mover.gameObject;

                // Si estaba inactivo, lo activamos temporalmente
                bool wasActive = go.activeSelf;
                if (!wasActive)
                    go.SetActive(true);

                // Ejecutar recursivamente el update
                mover.UpdateFinalPositionRotation(sender, obj);

                // Restaurar el estado original
                if (!wasActive)
                    go.SetActive(false);
            }
        }
        FinalPosition = transform.position;
        FinalRotation = transform.rotation.eulerAngles;
    }

    void sarasa()
    {
        Content.SetActive(false);
    }

    public void SetToReplace() => toReplece = true;

    public bool GetIsAUpdatedPhoto() { return IsAUpdatedPhoto; }

    public void MoveToPlacedPos(Component sender, object obj)
    {
        if (isOutOfBoard) return;
        if (conditions == null)
        {
            Debug.Log(transform.parent.parent.parent.gameObject.name);
            return;
        }
        
        try
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
        catch (NullReferenceException e)
        {
            Debug.LogError($"Error: Una referencia es nula. Verifica si {transform.parent.parent.parent.gameObject.name} o algún otro objeto es nulo. Detalles: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: Ocurrió una excepción inesperada. Detalles: {e.Message}");
        }

    } 

    public void MoveToTakeOutPos(Component sender, object obj)
    {
        if (!isPlaced) return;
        if (!conditions.IsOutOfBoard()) return;
        Transform _transform = (Transform)obj;
        Vector3 finalPos = _transform.position;
        Vector3 finalRot = new Vector3(_transform.rotation.x,0,0);

        transform.DOMoveY(finalPos.y, 0.5f * (transform.position.y *2)).SetEase(Ease.InOutSine);
        transform.DORotate(finalRot, 0.5f).SetEase(Ease.InExpo);

    }

    IEnumerator Delay(Vector3 InitPos)
    {
        yield return new WaitForSeconds(0f);
        transform.position = InitPos;

        Content.SetActive(true);

        transform.DOMove(FinalPosition, 1f).SetEase(Ease.InOutQuad);
        transform.DORotate(FinalRotation, 0.3f).SetEase(Ease.InOutCirc);
        OnPlaceInBoardSound?.Invoke(this, null);
         
    }

    public bool  GetIsPlaced()
    {
        return isPlaced;
    }

    public IPlacedOnBoard GetConditions()
    {
        return conditions;
    }

    public void SetIsOutOfBoard(bool x)
    {
        isOutOfBoard = x;
        if(x) isPlaced = false;
    }

}
