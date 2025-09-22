using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperStatesController : MonoBehaviour
{
    PaperMoveController.PaperState actualState = PaperMoveController.PaperState.first;
    [SerializeField] EnableDisableComponent blockCollider;

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
        yield return new WaitForSeconds(0.5f);
        blockCollider.SetComponentEnabled<Collider>(false);
    }
}
