using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyStateController : MonoBehaviour
{
    int stateNum = 0;
    [SerializeField] List<GameObject> States = new List<GameObject>();
    [SerializeField] GameObject MODEL;
    [SerializeField] BoxCollider eatingbtn;
    [Header("Candy1Settings")]
    [SerializeField] Material material1;
    [SerializeField] List<GameObject> insideCandies1 = new List<GameObject>();

    [Header("Candy1Settings")]
    [SerializeField] Material material2;
    [SerializeField] List<GameObject> insideCandies2 = new List<GameObject>();

    public void EateCandy(Component sender, object obj)
    {
        stateNum += 1;

        if(stateNum + 1 >= States.Count)
        {
            eatingbtn.enabled = true;
        }

        if (stateNum >= States.Count)
        {
            
            return;
        }

        foreach (GameObject state in States) state.SetActive(false);

        States[stateNum].SetActive(true);
    }

    public void InitializeCandy(ObjectToPrint candyType)
    {
        if(candyType == ObjectToPrint.Candy1)
        {
            assignMaterial(material1);
            ActiveInactiveObjects(insideCandies1, true);
            ActiveInactiveObjects(insideCandies2, false);
        }
        else if (candyType == ObjectToPrint.Candy2)
        {
            assignMaterial(material2);
            ActiveInactiveObjects(insideCandies1, false);
            ActiveInactiveObjects(insideCandies2, true);
        }
    }

    void assignMaterial(Material mat)
    {
        MeshRenderer[] children = MODEL.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer c in children) {
            c.material = mat;
        }
    }

    void ActiveInactiveObjects(List<GameObject> list, bool x)
    {
        foreach (GameObject obj in list)
        {
            if (obj != null) obj.SetActive(x);
        }
    }
}
