using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter_ExitTrigger : MonoBehaviour
{
    public bool _ExitTrigger;
    public bool _EnterTrigger;

    private void OnEnable()
    {
        _ExitTrigger = false;
        _EnterTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _EnterTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _ExitTrigger = true;

        }
    }
}
