using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardcodedTextReference : MonoBehaviour
{
    [TextArea(3,5)] [SerializeField] List<string> HardcodedText = new List<string>();

    public List<string> GetHardcodedTexts() => HardcodedText;

    public string GetHardcodedTextByIndex(int index = 1)
    {
        index--;

        if (index < 0 || index >= HardcodedText.Count)
            return string.Empty;

        return HardcodedText[index];
    }
}
