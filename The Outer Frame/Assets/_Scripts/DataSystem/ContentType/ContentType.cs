using System;
using System.Collections.Generic;
using UnityEngine;

abstract public class ContentType : DataType
{
    public Guid ID;

    private List<FindableWordData> findableWords = new();

    public IReadOnlyList<FindableWordData> FindableWords => findableWords;

    public void SetFindableWords(List<FindableWordData> words)
    {
        findableWords = words;
    }

    public bool HasPreProcessedWords()
    {
        return findableWords != null && findableWords.Count > 0;
    }

    public abstract string GetText();
}
