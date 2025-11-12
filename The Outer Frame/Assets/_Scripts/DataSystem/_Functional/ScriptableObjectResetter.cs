using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ScriptableObjectResetter : MonoBehaviour
{
    public static ScriptableObjectResetter instance { get; private set; }
    HashSet<IReseteableScriptableObject> SOlist = new HashSet<IReseteableScriptableObject>();
    [SerializeField] GameEvent OnChangeScene;

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

    public void ResetSOs(Component sender, object obj)
    {
        ResetAllScriptableObject();
    }

    public void RegisterScriptableObject(ScriptableObject _SO)
    {
        IReseteableScriptableObject SO = _SO as IReseteableScriptableObject;
        SOlist.Add(SO);
    }

    async Task ResetAllScriptableObject()
    {
        foreach (IReseteableScriptableObject so in SOlist)
        {
            so.ResetScriptableObject();
        }

        await Task.Delay(50);

        OnChangeScene?.Invoke(this, "Level1");
    }
}
