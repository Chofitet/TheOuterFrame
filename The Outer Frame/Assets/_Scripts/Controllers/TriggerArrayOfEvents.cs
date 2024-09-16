using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArrayOfEvents : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;
    [SerializeField] GameObject SomethingToPass;
    [SerializeField] string SomeStringToPass;

    public void TriggerEvents()
    {

        foreach (GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, PassRigthValue());
        }

    }

    public void TriggerEventsWithEvent(Component sender, object obj)
    {
        foreach (GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, PassRigthValue());
        }
    }

    object PassRigthValue()
    {
        if (SomethingToPass) return SomeStringToPass;
        else if (SomeStringToPass != "") return SomeStringToPass;

        return null;

    }
}
