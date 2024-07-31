using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New event", menuName = "Events")]
public class GameEvent : ScriptableObject
{
	private HashSet<GameEventListener> Listeners = new HashSet<GameEventListener>();
	public void Invoke(Component sender, object data)
	{
		foreach (var listener in Listeners)
        {
			listener.Raise(sender,data);
        }
	}
	public void registerListener(GameEventListener listener) => Listeners.Add(listener);
	public void UnregisterListener(GameEventListener listener) => Listeners.Remove(listener);
}
