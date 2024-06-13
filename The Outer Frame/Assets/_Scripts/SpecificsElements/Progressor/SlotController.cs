using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotController : MonoBehaviour
{
    [SerializeField] TMP_Text Wordtxt;
    [SerializeField] TMP_Text Actiontxt;


    
    public void initParameters(string word, string action) 
    {
        Wordtxt.text = word;
        Actiontxt.text = action;
    }

    public void ActionWasDone()
    {
        Wordtxt.text = "the action has already been done";
        Invoke("AbortAction", 5);
    }

    public void AbortAction()
    {
        Destroy(gameObject);
    }
}
