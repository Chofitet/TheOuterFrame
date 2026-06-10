using System;
using System.Collections.Generic;
using UnityEngine;

public class DatatService : MonoBehaviour
{
    [SerializeField] DataDirectory directory;

    public static DatatService instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one DataService in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        directory.Initialize();
    }


    public DataDirectory GetDirectory()
    {
       return directory;
    }


    public  void MarkDirty(DataType data)
    {
        directory.AddToModifyData(data);
    }
}
