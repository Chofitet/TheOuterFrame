using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class ColliderButtonCallEvent : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;
    [SerializeField] bool DisabledInTouch;
    [SerializeField] GameObject SomethingToPass;
    [SerializeField] float FloatToPass;
    bool isInactive;

    private void OnMouseUpAsButton()
    {
        triggerEvents();
    }


    void triggerEvents()
    {
        if (isInactive) return;
        foreach (GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, Something());
        }

        if (DisabledInTouch) GetComponent<BoxCollider>().enabled = false;
    }
    public void EnableBTN(Component sender, object obj)
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    private object Something()
    {
        if (SomethingToPass != null) return SomethingToPass;
        else if (FloatToPass != 0) return FloatToPass;

        return null;
    }

    public void Disable(Component sender, object obj)
    {
        isInactive = true;
    }

    public void TriggerEvents(Component sender, object obj)
    {
        triggerEvents();
    }
}
