using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Asset Directory")]
public class DataDirectory : ScriptableObject
{
    [SerializeField] private List<DataType> allData = new();

    private Dictionary<Guid, DataType> datasById = new();

    public void Initialize()
    {
        datasById.Clear();

        foreach (var data in allData)
        {
            if (data == null) continue;
            datasById[data.ID] = data;
        }
    }

    public bool Contains(DataType data) => allData.Contains(data);

    public bool Contains(Guid id) => datasById.ContainsKey(id);

    public void Add(DataType data)
    {
        if (!allData.Contains(data))
            allData.Add(data);

        datasById[data.ID] = data;
    }

    public void Remove(DataType data)
    {
        allData.Remove(data);
        datasById.Remove(data.ID);
    }

    public bool CleanupNullReferences()
    {
        bool removedSomething = false;

        for (int i = allData.Count - 1; i >= 0; i--)
        {
            if (allData[i] == null)
            {
                allData.RemoveAt(i);
                removedSomething = true;
            }
        }

        if (removedSomething)
        {
            Initialize();
        }

        return removedSomething;
    }

    public void Clear()
    {
        allData.Clear();
        datasById.Clear();
    }

    public IReadOnlyDictionary<Guid, DataType> GetDictionary()
        => datasById;
}
