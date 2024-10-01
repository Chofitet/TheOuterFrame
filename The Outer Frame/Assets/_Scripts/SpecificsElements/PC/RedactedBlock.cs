using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RedactedBlock : MonoBehaviour
{

    public void Initialization()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.localPosition = rectTransform.localPosition + new Vector3(-1.288529f, 0.327f, 0);
    }
}
