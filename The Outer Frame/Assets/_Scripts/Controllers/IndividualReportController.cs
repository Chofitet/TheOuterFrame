using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualReportController : MonoBehaviour
{
    bool isComplete;
    [SerializeField] GameEvent UpdatePCDatabase;
    public void SetType(bool x)
    {
        if (x)
        {
            isComplete = x;
        }
    }

    public void FinishReport()
    {
        if (isComplete) UpdatePCDatabase?.Invoke(this, gameObject);
        Destroy(gameObject);
    }
}
