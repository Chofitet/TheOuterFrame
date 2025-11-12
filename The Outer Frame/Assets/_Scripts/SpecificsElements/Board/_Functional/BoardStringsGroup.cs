using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStringsGroup : MonoBehaviour
{
    [SerializeField] List<GroupOfString> stringGroups = new List<GroupOfString>();

    public bool CheckIfOtherStringArePlaced(GameObject String)
    {
        GroupOfString actualGroup = null;

        //encontrar el grupo al que pertenece
        foreach(GroupOfString group in stringGroups)
        {
            if (group.CheckIfAreInList(String))
            {
                actualGroup = group;
                break;
            }
        }

        //chequear en si otra cuerda del grupo ya está puesta
        if (actualGroup != null)
        {
            foreach(GameObject conection in actualGroup.strings)
            {
                if (conection == null) continue;

                StringConnectionController stringController = conection.GetComponent<StringConnectionController>();
                if (stringController == null) continue;
                if (conection == String) continue;

                if(stringController.GetIsConnected()) return true;
            }
        }

        return false;

    }
}


[Serializable]

public class GroupOfString
{
    public string name;
    public List<GameObject> strings = new List<GameObject>();

    public bool CheckIfAreInList(GameObject String)
    {
        foreach (GameObject go in strings)
        {
            if(go == String) return true;
        }

        return false;
    }

}