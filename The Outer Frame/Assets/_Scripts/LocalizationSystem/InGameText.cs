using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Content/InGame Text")]
public class InGameText : ContentType
{
    [SerializeField, TextArea(3, 10)]
    private string text;

    public override string GetText() => text;

    public override string GetTextSecundary() => string.Empty;

    public void SetText(string newText)
    {
        if (text == newText) return;

        text = newText;

        if (DatatService.instance != null)
            MarkDirty();
    }
}
