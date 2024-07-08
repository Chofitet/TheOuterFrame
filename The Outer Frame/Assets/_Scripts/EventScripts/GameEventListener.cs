using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }
public class GameEventListener : MonoBehaviour
{
    [SerializeField] bool IsDesactive;
    [SerializeField] GameEvent TriggerEvent;
    [SerializeField] CustomGameEvent Event;

    private void Awake() => TriggerEvent.registerListener(this);

    private void OnDisable() => TriggerEvent.UnregisterListener(this);

    public void Raise(Component sender, object data)
    {
        if (IsDesactive) return;
        Event.Invoke(sender, data);
    }

    public void ActiveListener(Component sender ,object var)
    {
        IsDesactive = false;
    }

}
