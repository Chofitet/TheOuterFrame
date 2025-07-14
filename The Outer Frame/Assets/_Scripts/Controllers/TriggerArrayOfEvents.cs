using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArrayOfEvents : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;
    [SerializeField] float delay;
    [SerializeField] GameObject SomethingToPass;
    [SerializeField] string SomeStringToPass;

    public void TriggerEvents()
    {
        if (delay != 0)
        {
            Invoke("CallWithDelay", delay);
            return;
        }

        foreach (GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, PassRigthValue());
        }

    }

    public void TriggerEventsWithEvent(Component sender, object obj)
    {
        if (delay != 0)
        {
            Invoke("CallWithDelay", delay);
            return;
        }

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

    public void ChangeSomeStringToPass(string x)
    {
        SomeStringToPass = x;
    }

    void CallWithDelay()
    {
        TriggerEvents();
    }
}
