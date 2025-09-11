using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperStatesController : MonoBehaviour
{
    PaperMoveController.PaperState actualState = PaperMoveController.PaperState.first;

    public void SetPaperState(PaperMoveController.PaperState newState)
    {
        actualState = newState;
    }

    public PaperMoveController.PaperState GetPaperState()
    {
        return actualState;
    }
}
