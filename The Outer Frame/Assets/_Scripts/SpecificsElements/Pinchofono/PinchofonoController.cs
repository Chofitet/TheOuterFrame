using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchofonoController : MonoBehaviour
{
    [SerializeField] GameObject CallTranscriptionPrefab;
    [SerializeField] Transform InstanciateSpot;

    CallType CallToPrint;

    public void SetCallToPrint(Component sender, object obj)
    {
        CallType call = (CallType)obj;
        
        
        CallToPrint = call;
    }

    public void PrintCall(Component sender, object obj)
    {
        GameObject aux = Instantiate(CallTranscriptionPrefab, InstanciateSpot);
        aux.GetComponent<TranscriptionCallController>().Inicialization(CallToPrint);
    }
}
