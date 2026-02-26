using System;
using System.Collections.Generic;
using UnityEngine;

abstract public class ContentType : DataType
{
    public Guid ID;

    private List<FindableWordData> findableWords;

    public IReadOnlyList<FindableWordData> FindableWords => findableWords;

    public void SetFindableWords(List<FindableWordData> words)
    {
        findableWords = words;
    }
    public abstract string GetText();

    public abstract string GetTextSecundary();
}
