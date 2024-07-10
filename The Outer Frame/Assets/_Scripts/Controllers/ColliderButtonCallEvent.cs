using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class ColliderButtonCallEvent : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;
    [SerializeField] bool DisabledInTouch;
    [SerializeField] GameObject SomethingToPass;

    private void OnMouseUpAsButton()
    {
        foreach(GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, SomethingToPass);
        }

        if (DisabledInTouch) GetComponent<BoxCollider>().enabled = false;
    }

    public void EnableBTN(Component sender, object obj)
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
