using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;



using UnityEditor;
#if UNITY_EDITOR

[InitializeOnLoad]
#endif
public static class DataServiceEditor 
{

    private const string directoryPath = "Assets/ScriptableObjects/Directory.asset";
    private const string dataRootFolder = "Assets/ScriptableObjects";

    private static DataDirectory directory;

    static DataServiceEditor()
    {
        directory = LoadRuntime();
#if UNITY_EDITOR
        directory = LoadOrCreateDirectoryEditor();
#endif
        directory.Initialize();
    }

    private static DataDirectory LoadRuntime()
    {
        if (directory != null)
            return directory;

        directory = Resources.Load<DataDirectory>(directoryPath);

        return directory;
    }
#if UNITY_EDITOR
    private static DataDirectory LoadOrCreateDirectoryEditor()
    {
        var dir = AssetDatabase.LoadAssetAtPath<DataDirectory>(directoryPath);

        if (dir == null)
        {
            dir = ScriptableObject.CreateInstance<DataDirectory>();
            AssetDatabase.CreateAsset(dir, directoryPath);
            AssetDatabase.SaveAssets();
        }

        return dir;
    }

    // ===============================
    // REBUILD MANUAL
    // ===============================
    public static void RebuildAllDirectory()
    {
        directory.Clear();

        string[] guids = AssetDatabase.FindAssets("t:DataType", new[] { dataRootFolder });

        HashSet<Guid> usedIds = new();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<DataType>(path);
            if (data == null) continue;

            if (data.ID == Guid.Empty || usedIds.Contains(data.ID))
            {
                data.ID = Guid.NewGuid();
                EditorUtility.SetDirty(data);
            }

            usedIds.Add(data.ID);
            directory.Add(data);
        }

        EditorUtility.SetDirty(directory);
        AssetDatabase.SaveAssets();

        Debug.Log($"Rebuild complete. Total: {usedIds.Count}");
    }

    // INCREMENTAL REGISTER
    public static void Register(DataType data)
    {
        if (data == null) return;

        if (data.ID == Guid.Empty)
        {
            data.ID = Guid.NewGuid();
            EditorUtility.SetDirty(data);
        }

        if (directory.Contains(data.ID) &&
            directory.GetDictionary()[data.ID] != data)
        {
            data.ID = Guid.NewGuid();
            EditorUtility.SetDirty(data);
        }

        directory.Add(data);
        EditorUtility.SetDirty(directory);
    }

    public static void CleanupDeleted()
    {
        directory.CleanupNullReferences();
    }

    public static void Remove(DataType data)
    {
        if (data == null) return;

        directory.Remove(data);
        EditorUtility.SetDirty(directory);
    }
#endif
    public static DataType Get(Guid id)
    {
        return directory.GetDictionary().TryGetValue(id, out var data)
            ? data
            : null;
    }
}

