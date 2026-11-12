using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizedTextComponent : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private InGameText content;
    [SerializeField] string AssetName;

    public TMP_Text TextField => textField;
    public InGameText Content => content;

    public void AssignContent(InGameText newContent)
    {
        content = newContent;
    }

    private void Reset()
    {
        textField = GetComponent<TMP_Text>();
    }

    public string GetAssetName()
    {
        if (AssetName != null) return AssetName;
        else return "";
    }
}
