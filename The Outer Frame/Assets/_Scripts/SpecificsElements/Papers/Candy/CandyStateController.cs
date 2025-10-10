using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyStateController : MonoBehaviour
{
    int stateNum = 0;
    [SerializeField] List<GameObject> States = new List<GameObject>();

    public void EateCandy(Component sender, object obj)
    {
        stateNum += 1;

        if (stateNum >= States.Count) return;

        foreach (GameObject state in States) state.SetActive(false);

        States[stateNum].SetActive(true);
    }
}
