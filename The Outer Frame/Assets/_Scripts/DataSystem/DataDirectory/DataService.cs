using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DatatService : MonoBehaviour
{
    [SerializeField] DataDirectory directory;
    private List<AnalyticsEntry> analyticsMarks = new();
    public static DatatService instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one DataService in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        directory.Initialize();
    }

    public void AddAnalyticMark(AnalyticsEntry entry)
    {
        analyticsMarks.Add(entry);
    }

    public void EndGame(Component sender, object obj)
    {
        List<AnalyticsEntry> entries = new();

        // Analytics provenientes de ScriptableObjects
        foreach (DataType data in directory.GetModifyData().Distinct())
        {
            if (data is IAnalytics analytics)
            {
                entries.Add(analytics.GetAnalyticsData());
            }
        }

        // Analytics que son marcas
        entries.AddRange(analyticsMarks);

        AnalyticsExporter.Export(entries);
    }

    public DataDirectory GetDirectory()
    {
       return directory;
    }


    public  void MarkDirty(DataType data)
    {
        directory.AddToModifyData(data);
    }
}
