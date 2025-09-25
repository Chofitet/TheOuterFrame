
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Linq;

public static class AddSOAddressables
{
    private const string GroupName = "Common";
    private const string LabelName = "resettable";

    [MenuItem("Tools/Addressables/Mark All Resettable SO")]
    public static void MarkAllResettablesAsAddressable()
    {
        // Obtener settings de Addressables
        var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        if (settings == null)
        {
            Debug.LogError("No se encontró AddressableAssetSettings. Creá uno en Window → Asset Management → Addressables → Groups");
            return;
        }

        // Buscar grupo "Common"
        var group = settings.groups.FirstOrDefault(g => g != null && g.Name == GroupName);
        if (group == null)
        {
            Debug.LogError($"No se encontró el grupo '{GroupName}'. Crealo manualmente en Addressables Groups.");
            return;
        }

        // Buscar todos los ScriptableObjects
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        int counter = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (so is IReseteableScriptableObject)
            {
                // Crear o mover al grupo
                var entry = settings.FindAssetEntry(guid);
                if (entry == null)
                    entry = settings.CreateOrMoveEntry(guid, group);
                else
                    settings.MoveEntry(entry, group);

                // Agregar label
                entry.SetLabel(LabelName, true);
                counter++;


            }
        }

       
        Debug.Log($"Marcados {counter} ScriptableObjects como Addressables en el grupo '{GroupName}' con label '{LabelName}'.");
    }
}
#endif
