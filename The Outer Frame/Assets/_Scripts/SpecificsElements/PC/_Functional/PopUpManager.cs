using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] List<PopUpPrefabs> popUpPrefabList;
    List<GameObject> popUpList;
    RectTransform WindowRect;
    [SerializeReference] float EdgeMargin = 2;

    private void Start()
    {
        WindowRect = GetComponent<RectTransform>();
    }

    public void OnInstanciatePopUp(Component sender, object obj)
    {
        IPopUp popUpData = (IPopUp) obj;

        if (popUpData.PopUpType == PopUpType.None) return;

        GameObject popUpPrefab = GetPopUpType(popUpData.PopUpType);

        GameObject Instance = Instantiate(popUpPrefab, transform,false);

        
        PopUpController popUpController = Instance.GetComponent<PopUpController>();

        Instance.transform.localPosition = GetRandomPositionOnCanvas(popUpController);

        popUpController.Initialize(popUpData.PopupText, GetComponentInParent<RectTransform>());
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

    Vector3 GetRandomPositionOnCanvas(PopUpController popUp)
    {
        Vector2 popupSize = popUp.GetPopUpSize();
        Vector2 windowSize = WindowRect.sizeDelta;

        float MarginInX = windowSize.x - popupSize.x - EdgeMargin;
        float MarginInY = - windowSize.y + popupSize.y + EdgeMargin;

        Vector3 randomPosition = Vector2.zero;

        randomPosition.x = UnityEngine.Random.Range(EdgeMargin, MarginInX);
        randomPosition.y = UnityEngine.Random.Range(-EdgeMargin, MarginInY);
        randomPosition.z = 0;

        return randomPosition;
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