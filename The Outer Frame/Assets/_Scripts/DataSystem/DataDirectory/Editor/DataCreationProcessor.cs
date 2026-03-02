using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

public class DataCreationProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
    string[] movedFromAssetPaths)
    {

        foreach (var path in importedAssets)
        {
            var data = AssetDatabase.LoadAssetAtPath<DataType>(path);
            if (data != null) DataServiceEditor.Register(data);
        }

        foreach (var path in deletedAssets)
        {
            if (path.StartsWith("Assets/ScriptableObjects"))
            {
                DataServiceEditor.CleanupDeleted();
                break;
            }
        }

    }
}
