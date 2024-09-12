using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualCallController : MonoBehaviour
{
    bool isComplete;
    CallType call;
    [SerializeField] GameEvent UpdatePCDatabase;
    [SerializeField] GameEvent OnDescartReport;
    [SerializeField] GameEvent OnBackToGeneralView;
    public void SetType(bool x, CallType _call)
    {
        call = _call;
        isComplete = x;
    }

    public void FinishReport()
    {
        if (isComplete)
        {
            UpdatePCDatabase?.Invoke(this, gameObject);
        }
        else
        {
            OnDescartReport?.Invoke(this, null);
        }
        StartCoroutine(delay());
        OnBackToGeneralView?.Invoke(this, null);
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
