using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontSizeAdjustToOneLine : MonoBehaviour
{
    [SerializeField] float maxFontSize = 60;
    [SerializeField] float minFontSize = 10;
    [SerializeField] bool updateEveryFrame = false;

    private TextMeshProUGUI tmpText;
    private RectTransform rect;

    float originalFontSize;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();

        originalFontSize = tmpText.fontSize;
    }

    private void Start()
    {
        AdjustFontSize();
    }

    private void Update()
    {
        if (updateEveryFrame)
            AdjustFontSize();
    }

    public void AdjustFontSize()
    {
        if (tmpText == null) return;

        tmpText.enableAutoSizing = false; // Desactivamos el autosize de TMP
        tmpText.fontSize = originalFontSize;

        Debug.Log(tmpText.text);

        tmpText.ForceMeshUpdate();

        // Reducir tamaño hasta que quepa en el ancho y no se rompa en varias líneas
        while ((tmpText.preferredWidth > rect.rect.width || tmpText.textInfo.lineCount > 1)
               && tmpText.fontSize > minFontSize)
        {
            tmpText.fontSize -= 0.1f;
           // tmpText.ForceMeshUpdate();
        }
    }

    public void ResetFontSize()
    {
        tmpText.fontSize = originalFontSize;
    }
}
