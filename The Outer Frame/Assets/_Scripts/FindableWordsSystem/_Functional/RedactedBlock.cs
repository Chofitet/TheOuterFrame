using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RedactedBlock : MonoBehaviour
{
    [SerializeField] TMP_Text m_Text;
    public void Initialization(string redactedText)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.localPosition = rectTransform.localPosition + new Vector3(-1.288529f, 0.327f - 0.1f, 0);

        if (redactedText == "RE")
        {
            m_Text.text = "XXX";
            rectTransform.sizeDelta = new Vector2(4.49f, rectTransform.sizeDelta.y);
        }
        else if (redactedText == "REDA")
        {
            m_Text.text = "XXXXX";
            rectTransform.sizeDelta = new Vector2(7.64f, rectTransform.sizeDelta.y);
        }
        else if (redactedText == "REDACTED")
        {
            rectTransform.sizeDelta = new Vector2(13.1f, rectTransform.sizeDelta.y);
            m_Text.text = "XXXXXXXXX";
        }
        else if (redactedText == "REDACTEDTO")
        {
            m_Text.text = "XXXXXXXXXX";
            rectTransform.sizeDelta = new Vector2(15.86f, rectTransform.sizeDelta.y);
        }

        transform.localScale = Vector3.one;

        //Invoke("CheckIfInsideRect", 0.1f);
    }

   /* public void CheckIfInsideRect()
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
    }*/
}
