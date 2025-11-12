using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRigthPosInFile : MonoBehaviour
{
   public void ForceRightPos(Component sender, object obj)
    {
        foreach(Transform paper in GetChilds())
        {
            paper.localPosition = new Vector3 (0, paper.position.y, 0);
        }
    }

    List<Transform> GetChilds()
    {
        Transform[] Allchilds = transform.GetComponentsInChildren<Transform>();
        List<Transform> childs = new List<Transform>();

        foreach(Transform child in Allchilds)
        {
            if(child.GetComponent<PaperStatesController>()) childs.Add(child);
        }

        return childs;
    }
}
