using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HardcodedTextReference))]
public class LocalizedTextComponentList : MonoBehaviour
{
    [SerializeField] private HardcodedTextReference reference;
    [SerializeField] private List<InGameText> contents = new();

    public HardcodedTextReference Reference => reference;
    public List<InGameText> Contents => contents;

    private void Reset()
    {
        reference = GetComponent<HardcodedTextReference>();
    }

    public void AssignContent(int index, InGameText content)
    {
        while (contents.Count <= index)
            contents.Add(null);

        contents[index] = content;
    }

    public string GetTextByIndex(int index = 1)
    {
        index--;

        if (index < 0 || reference == null ||
            index >= reference.GetHardcodedTexts().Count)
            return string.Empty;

        if (index < contents.Count && contents[index] != null)
            return contents[index].GetText();

        return reference.GetHardcodedTextByIndex(index + 1);
    }
}
