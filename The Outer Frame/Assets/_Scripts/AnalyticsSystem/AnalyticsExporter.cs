using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnalyticsExporter : MonoBehaviour
{
    public static void Export(List<AnalyticsEntry> entries)
    {
        AnalyticsFile analyticsFile = new AnalyticsFile
        {
            Entries = entries
        };

        string analyticsDirectory = Path.Combine(
            Application.persistentDataPath,
            "Analytics"
        );

        Directory.CreateDirectory(analyticsDirectory);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string fileName = $"Run_Analytics_{timestamp}.json";

        string path = Path.Combine(
            analyticsDirectory,
            fileName
        );

        string json = JsonUtility.ToJson(analyticsFile, true);

        File.WriteAllText(path, json);

        Debug.Log($"Analytics exported to: {path}");
    }
}


[Serializable]
public class AnalyticsFile
{
    public List<AnalyticsEntry> Entries = new();
}

