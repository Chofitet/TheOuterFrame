using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArrayOfEvents : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;
    [SerializeField] GameObject SomethingToPass;

    public void TriggerEvents()
    {
        foreach (GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, SomethingToPass);
        }

    }
}
