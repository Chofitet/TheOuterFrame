using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualCallController : MonoBehaviour
{
    bool isComplete;
    CallType call;
    WordData word;
    [SerializeField] GameEvent UpdatePCDatabase;
    [SerializeField] GameEvent OnDescartReport;
    [SerializeField] GameEvent OnBackToGeneralView;
    public void SetType(bool x, CallType _call)
    {
        call = _call;
        word = call.GetWord();
        isComplete = x;
    }

    public void FinishReport()
    {
        if (isComplete)
        {
            UpdatePCDatabase?.Invoke(this, word);
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
