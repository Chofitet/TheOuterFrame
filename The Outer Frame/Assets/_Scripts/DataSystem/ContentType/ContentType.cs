using System;
using System.Collections.Generic;
using UnityEngine;

abstract public class ContentType : DataType, ILocalizable
{
    
    string primaryText;
    string secondaryText;

    private List<FindableWordData> findableWords;

    public IReadOnlyList<FindableWordData> FindableWords => findableWords;

    [SerializeField]
    private List<LocalizedText> localizedTexts =
        new List<LocalizedText>();

    public void SetFindableWords(List<FindableWordData> words)
    {
        findableWords = words;
    }
    public abstract string GetText();

    public abstract string GetTextSecundary();

    public virtual IEnumerable<LocalizationEntry> GetLocalizationEntries()
    {
        yield break;
    }
    public string GetLocalizedText(string key, string language)
    {
        LocalizedText entry = localizedTexts.Find(
            x => x.key == key && x.language == language
        );

        return entry?.value;
    }

    public void SetLocalizedText(string key, string language, string value)
    {
        LocalizedText entry = localizedTexts.Find(
            x => x.key == key && x.language == language
        );

        if (entry == null)
        {
            entry = new LocalizedText
            {
                key = key,
                language = language
            };

            localizedTexts.Add(entry);
        }

        entry.value = value;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (GetText() != primaryText || GetTextSecundary() != secondaryText) 
        {
            DataServiceEditor.AddToNeedsReprocess(this);
        }

        primaryText = GetText();
        secondaryText = GetTextSecundary();
#endif
    }

   
}
