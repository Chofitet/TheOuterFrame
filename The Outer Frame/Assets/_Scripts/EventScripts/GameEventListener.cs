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
    [SerializeField] float DelayCall = 0;

    private void Awake() => TriggerEvent.registerListener(this);

    private void OnDisable() => TriggerEvent.UnregisterListener(this);

    public void Raise(Component sender, object data)
    {
        if (IsDesactive) return;
        if(DelayCall == 0)
        {
            Event.Invoke(sender, data);
        }
        else StartCoroutine(Delay(sender,data));
    }

    public void ActiveListener(Component sender ,object var)
    {
        IsDesactive = false;
    }

    public void DesactiveListener(Component sender, object var)
    {
        IsDesactive = true;
    }

    IEnumerator Delay(Component sender, object data)
    {
        yield return new WaitForSeconds(DelayCall);
        Event.Invoke(sender, data);
    }

}
