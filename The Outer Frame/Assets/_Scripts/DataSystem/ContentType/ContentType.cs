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
