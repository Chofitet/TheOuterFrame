using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVNewTypeProperties : MonoBehaviour
{
    [SerializeField] List<TVNewPropertiesData> tVNewPropertiesData = new List<TVNewPropertiesData>();

    public TVNewPropertiesData GetTVNewPropertyData(NewType type)
    {
        foreach(TVNewPropertiesData data in tVNewPropertiesData)
        {
            if(data.Type == type)
            {
                return data;
            }
        }

        return tVNewPropertiesData[0];
    }
}

public enum NewType
{
    Custom, // Con este seleccionado, se toma lo que hay en la caja de texto "Custom Aclaration Alelert"
    RandomNews, // no cambiar ni borrar (hace que por default las randoms no muestren nada)
    Evidence,
    //agragar cuantos quieras
}

[Serializable]
public class TVNewPropertiesData
{
    public NewType Type;
    [TextArea(1,3)]public string TextInAlertScreen;
}