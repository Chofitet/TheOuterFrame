using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] List<PopUpPrefabs> popUpPrefabList;
    List<GameObject> popUpInstances = new List<GameObject>();
    RectTransform WindowRect;
    [SerializeField] int MaxToStack;
    [SerializeField] Vector3 PopUpOffset;
    [SerializeField] float EdgeMargin = 2;
    [SerializeField] RectTransform instantiationPoint;

    private void Start()
    {
        WindowRect = GetComponent<RectTransform>();
    }

    public void OnInstanciatePopUp(Component sender, object obj)
    {
        removeClosedPopUp();

        IPopUp popUpData = (IPopUp) obj;

        if (popUpData.PopUpType == PopUpType.None) return;

        GameObject popUpPrefab = GetPopUpType(popUpData.PopUpType);

        GameObject Instance = Instantiate(popUpPrefab, instantiationPoint.transform, false);

        PopUpController popUpController = Instance.GetComponent<PopUpController>();

        popUpController.Initialize(popUpData.PopupText, GetComponentInParent<RectTransform>());

        Instance.transform.localPosition = GetInstancePosition();

        popUpInstances.Add(Instance);

        Instance.transform.SetParent(transform);
    }


    GameObject GetPopUpType(PopUpType type)
    {
        foreach(PopUpPrefabs prefab in popUpPrefabList)
        {
            if(prefab.popUpType == type)
            return prefab.popUpPrefab;
        }
        return null;
    }

    Vector3 GetInstancePosition()
    {
        Vector3 instancePos = Vector3.zero;

        int count = popUpInstances.Count;

        if (count < MaxToStack)
        {
            instancePos = PopUpOffset * count;
        }
        else
        {
            instancePos = PopUpOffset * (MaxToStack - 1);
        }

        return instancePos;
    }

    void removeClosedPopUp()
    {
        popUpInstances.RemoveAll(s => s == null);
    }
}


public interface IPopUp
{
    string PopupText { get; }
    PopUpType PopUpType { get; }
}


[Serializable]
public class PopUpPrefabs
{
    public GameObject popUpPrefab;
    public PopUpType popUpType;
}

public enum PopUpType
{
    None,
    common,
    good,
    bad,
    warning
}