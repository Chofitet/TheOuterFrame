using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }
public class GameEventListener : MonoBehaviour
{
    [SerializeField] GameEvent TriggerEvent;
    [SerializeField] CustomGameEvent Event;

    private void OnEnable() => TriggerEvent.registerListener(this);

    private void OnDisable() => TriggerEvent.UnregisterListener(this);

    public void Raise(Component sender, object data ) => Event.Invoke(sender, data);
}
