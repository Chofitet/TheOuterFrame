using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranscriptionCallController : MonoBehaviour
{
    [SerializeField] TMP_Text txtCall;

    public void Inicialization(CallType call)
    {
        if(!call)
        {
            txtCall.text = "No hemos encontrado ninguna llamada";
            return;
        }

        txtCall.text = call.GetDialogue();
        FindableWordsManager.FWM.InstanciateFindableWord(txtCall);
    }

    public void DestroyTranscription(Component sender, object obj)
    {
        Invoke("delay", 0.1f);
    }

    void delay()
    {
        Destroy(gameObject);
    }

}
