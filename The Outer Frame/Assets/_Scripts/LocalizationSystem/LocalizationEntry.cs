using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationEntry
{
    public string Key;
    public string PropertyPath;
    public string English;
    public int WordsCount;
    public LocalizationEntry(string key, string propertyPath, string english)
    {
        Key = key;
        PropertyPath = propertyPath;
        English = english;
    }
}

[System.Serializable]
public class LocalizedText
{
    public string key;
    public string language;
    [TextArea] public string value;
}

public static class LocalizationSettings
{
    public const string CurrentLanguage = "es";
}

public interface ILocalizable
{
    IEnumerable<LocalizationEntry> GetLocalizationEntries();

    string GetLocalizedText(string key, string language);
    void SetLocalizedText(string key, string language, string value);

}
