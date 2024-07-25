using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCCallController : MonoBehaviour
{
    [SerializeField] TMP_Text txt;
    [SerializeField] GameEvent OnPressPCCallBTN;

    string _input;

    public void Inicialization(CallType call)
    {
        txt.text = call.GetTimeWhenWasDone().ToString(); 
        _input = call.GetDialogue();
    }

    public void ShowPanelWithInput()
    {
        OnPressPCCallBTN?.Invoke(this, _input);
    }
}
