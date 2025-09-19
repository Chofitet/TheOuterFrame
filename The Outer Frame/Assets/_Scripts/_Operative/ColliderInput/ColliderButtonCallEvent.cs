using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class ColliderButtonCallEvent : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;
    [SerializeField] float delay;
    [SerializeField] bool DisabledInTouch;
    [SerializeField] float delayDisableInTouch;
    [SerializeField] GameObject SomethingToPass;
    [SerializeField] float FloatToPass;
    bool isInactive;

    private void OnMouseUpAsButton()
    {
        if (delay != 0)
        {
            Invoke("CallWithDelay", delay);
            return;
        }
        triggerEvents();
    }


    void triggerEvents()
    {
        if (isInactive) return;
        foreach (GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, Something());
        }

        if (DisabledInTouch) StartCoroutine(EnableDisableBTN(false, delayDisableInTouch));
    }
    public void EnableBTN(Component sender, object obj)
    {
        StartCoroutine(EnableDisableBTN(true, 0));
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

    void CallWithDelay()
    {
        triggerEvents();
    }

    IEnumerator EnableDisableBTN(bool x,float _delay)
    {
        yield return new WaitForSeconds(_delay);

        GetComponent<BoxCollider>().enabled = x;
    }
}
