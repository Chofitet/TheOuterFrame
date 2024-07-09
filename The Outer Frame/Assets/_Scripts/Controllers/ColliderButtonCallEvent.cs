using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class ColliderButtonCallEvent : MonoBehaviour
{
    [SerializeField] GameEvent[] EventsToTrigger;

    private void OnMouseUpAsButton()
    {
        foreach(GameEvent e in EventsToTrigger)
        {
            e?.Invoke(this, this);
        }
    }
}
