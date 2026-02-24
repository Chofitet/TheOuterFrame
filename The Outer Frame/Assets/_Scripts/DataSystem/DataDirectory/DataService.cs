using System;
using System.Collections.Generic;
using UnityEngine;

public class DatatService : MonoBehaviour
{
    Dictionary<Guid, DataType> directory;
    [SerializeField] string ID;

    void GetDirectory()
    {
       // directory = DataServiceEditor.datasById;
    }

    public DataType GetConfig(Guid IDguid)
    {
        if (directory == null)
            return null;

        if (directory.TryGetValue(IDguid, out var config))
            return config;

        return null;
    }

    public DataType GetConfig(string IDstring)
    {
        if (Guid.TryParse(IDstring, out var guid))
            return GetConfig(guid);

        return null;
    }
}
