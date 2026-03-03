using System;
using System.Collections.Generic;

abstract public class ContentType : DataType
{
    public Guid ID;

    string primaryText;
    string secondaryText;

    private List<FindableWordData> findableWords;

    public IReadOnlyList<FindableWordData> FindableWords => findableWords;

    public void SetFindableWords(List<FindableWordData> words)
    {
        findableWords = words;
    }
    public abstract string GetText();

    public abstract string GetTextSecundary();

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
