using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Asset Directory")]
public class DataDirectory : ScriptableObject
{
    [SerializeField] private List<DataType> allData = new();

    //[SerializeField] private List<DataType> NeedPreprocess = new();

    [SerializeField] private List<DataType> ModifyData = new();

    private Dictionary<Guid, DataType> datasById = new();

    public void Initialize()
    {
        datasById.Clear();
        ClearModifyData();
        foreach (var data in allData)
        {
            if (data == null) continue;
            datasById[data.ID] = data;
        }
    }

    public DataType GetById(Guid id)
    {
        datasById.TryGetValue(id, out var data);
        return data;
    }

    public DataType GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        if (!Guid.TryParse(id, out Guid guid))
            return null;

        if(datasById.TryGetValue(guid, out var data))
        return data;

        Debug.LogWarning($"No data found in Directory with ID: {id}");
        return null;
    }

    public bool Contains(DataType data) => allData.Contains(data);

    public bool Contains(Guid id) => datasById.ContainsKey(id);

    public List<T> GetAllOfType<T>() where T : ScriptableObject
    {
        return allData.OfType<T>().ToList();
    }
    public List<ContentType> GetAllContentTypes()
    {
        return allData.OfType<ContentType>().ToList();
    }
    public List<ReportType> GetAllReportTypes()
    {
        return allData.OfType<ReportType>().ToList();
    }
    public List<TVNewType> GetAllTVNewType()
    {
        return allData.OfType<TVNewType>().ToList();
    }
    public List<CallType> GetAllTranscriptType()
    {
        return allData.OfType<CallType>().ToList();
    }
    public List<DataBaseType> GetAllDBType()
    {
        return allData.OfType<DataBaseType>().ToList();
    }


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

    public void AddNeedPreprocess(DataType data)
    {
       /* if (!NeedPreprocess.Contains(data))
            NeedPreprocess.Add(data);*/
    }

    public void ClearPreprocess()
    {
        //NeedPreprocess.Clear();
    }

    public void AddToModifyData(DataType data)
    {
        ModifyData.Add(data);
    }

    public void ClearModifyData() => ModifyData.Clear();

    public List<DataType> GetModifyData() { return ModifyData; }

    public List<DataType> GetAllData() { return allData; }

   // public List<DataType> GetNeedReprocess() { return NeedPreprocess; }

    public IReadOnlyDictionary<Guid, DataType> GetDictionary()
        => datasById;
}
