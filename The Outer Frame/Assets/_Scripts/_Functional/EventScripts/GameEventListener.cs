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

    [Header("Data To Pass")]
    [SerializeField] float _float;
    [SerializeField] string _string;
    [SerializeField] ViewStates viewState;
    [SerializeField] GameEvent gameEvent;

    private void OnEnable() => TriggerEvent.registerListener(this);

    private void OnDisable() => TriggerEvent.UnregisterListener(this);

    bool isDelaying;
    public void Raise(Component sender, object data)
    {
        if(data == null)
        {
            if (_float != 0) data = _float;
            if (_string != "") data = _string;
            if (viewState != ViewStates.GeneralView) data = viewState;
            if(gameEvent) data = gameEvent;
        }

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
        isDelaying = true;
        yield return new WaitForSeconds(DelayCall);
        if (!isDelaying)
        {
            yield return null;
        }
        Event.Invoke(sender, data);
    }

    public GameEventListener(GameEvent triggerEvent, CustomGameEvent _event)
    {
        TriggerEvent = triggerEvent;
        Event = _event;
    }

    public void SetDelay(Component sender, object obj)
    {
        DelayCall = (float)obj;
    }
    public void CancelDelayCall(Component sender, object obj)
    {
        isDelaying = false;
    }
}
