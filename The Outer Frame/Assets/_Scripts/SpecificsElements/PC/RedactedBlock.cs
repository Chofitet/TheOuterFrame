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

        //Invoke("CheckIfInsideRect", 0.1f);
    }

    public void CheckIfInsideRect()
    {
        RectTransform rectTransform = GetComponentInParent<RectTransform>();

        Vector3 localPosition = rectTransform.InverseTransformPoint(transform.position);

        if (!IsInsideRect(localPosition, rectTransform))
        {
            Debug.Log("destroyed block");
            Destroy(gameObject);
        }
    }

    private bool IsInsideRect(Vector3 localPosition, RectTransform rectTransform)
    {
        return localPosition.x >= rectTransform.rect.xMin &&
               localPosition.x <= rectTransform.rect.xMax &&
               localPosition.y >= rectTransform.rect.yMin &&
               localPosition.y <= rectTransform.rect.yMax;
    }
}
