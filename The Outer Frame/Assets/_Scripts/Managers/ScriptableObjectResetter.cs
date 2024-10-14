using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectResetter : MonoBehaviour
{
    public static ScriptableObjectResetter instance { get; private set; }
    HashSet<IReseteableScriptableObject> SOlist = new HashSet<IReseteableScriptableObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void RegisterScriptableObject(ScriptableObject _SO)
    {
        IReseteableScriptableObject SO = _SO as IReseteableScriptableObject;
        SOlist.Add(SO);
    }

    public void OnRetryLevel(Component sender, object obj)
    {
        ResetAllScriptableObject();
    }

    void ResetAllScriptableObject()
    {
        foreach (IReseteableScriptableObject so in SOlist)
        {
            so.ResetScriptableObject();
        }
    }
}
