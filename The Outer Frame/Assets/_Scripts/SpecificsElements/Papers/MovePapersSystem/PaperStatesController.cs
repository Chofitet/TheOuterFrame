using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperStatesController : MonoBehaviour
{
    PaperMoveController.PaperState actualState = PaperMoveController.PaperState.first;
    [SerializeField] EnableDisableComponent blockCollider;
    [SerializeField] GraphicRaycaster raycaster;

    public void SetPaperState(PaperMoveController.PaperState newState)
    {
        actualState = newState;
        StartCoroutine(BlockRayCast());
    }

    public PaperMoveController.PaperState GetPaperState()
    {
        return actualState;
    }

    IEnumerator BlockRayCast()
    {
        blockCollider.SetComponentEnabled<Collider>(true);
        raycaster.enabled = false;
        yield return new WaitForSeconds(0.5f);
        blockCollider.SetComponentEnabled<Collider>(false);
        if(actualState == PaperMoveController.PaperState.Taken) raycaster.enabled = true;
    }

    public void DisableRayCast()
    {
        raycaster.enabled = false;
    }
}
