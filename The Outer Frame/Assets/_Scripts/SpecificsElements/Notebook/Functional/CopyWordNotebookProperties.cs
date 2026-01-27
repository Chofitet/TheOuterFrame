using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyWordNotebookProperties : MonoBehaviour
{
    public float height;

    private void Start()
    {
        height = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.height;
    }
}
