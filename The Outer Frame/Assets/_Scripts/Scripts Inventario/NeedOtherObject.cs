using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedOtherObject : MonoBehaviour
{
    [SerializeField] ObjectType typeObject;
    public bool objectUsed;
    private NeedOtherObject ThisScript;
    MonoBehaviour[] scripts;
    [SerializeField] string[] NoDesenableScripts;
    public bool NeedAndGivesObject;
    private void Awake()
    {
        ThisScript = GetComponent<NeedOtherObject>();
    }
    public ObjectType GetObjectType()
    {
        return typeObject;
    }

    private void Start()
    {
        scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != ThisScript)
            {
                script.enabled = false;
            }

            foreach (string txt in NoDesenableScripts)
            {
                Debug.Log(script.ToString());
                if (script.ToString().Contains(txt))
                {
                    script.enabled = true;

                    Debug.Log(script.ToString() + " is active");
                }
               
            }
        }
    }
    public void ObjectUsed ()
    {
        objectUsed = true;
        
        foreach (MonoBehaviour script in scripts)
        {
            if (script != ThisScript)
            {
                script.enabled = true;
            }
        }
        
    }
    private void Update()
    {
        
    }

}
