using ClosedXML.Excel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LocalizationImporter : EditorWindow
{
    private DataDirectory directory;

    [MenuItem("Tools/Localization/Import Translations")]
    public static void OpenWindow()
    {
        GetWindow<LocalizationImporter>("Localization Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Localization Importer", EditorStyles.boldLabel);

        directory = (DataDirectory)EditorGUILayout.ObjectField(
            "Data Directory",
            directory,
            typeof(DataDirectory),
            false
        );

        GUILayout.Space(10);

        GUI.enabled = directory != null;

        if (GUILayout.Button("Import Spanish"))
        {
            ImportTranslations();
        }

        GUI.enabled = true;
    }

    private void ImportTranslations()
    {
        string path = EditorUtility.OpenFilePanel(
            "Select localization file",
            Application.dataPath,
            "xlsx,xlsm"
        );

        if (string.IsNullOrEmpty(path))
            return;

        // Reconstruimos el diccionario del directorio.
        directory.Initialize();

        int imported = 0;
        int missing = 0;
        int skipped = 0;

        try
        {
            using (var workbook = new XLWorkbook(path))
            {
                foreach (var worksheet in workbook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        string contentID = row.Cell(1).GetString();
                        string key = row.Cell(4).GetString();
                        string spanish = row.Cell(6).GetString();

                        if (string.IsNullOrWhiteSpace(contentID) ||
                            string.IsNullOrWhiteSpace(key) ||
                            string.IsNullOrWhiteSpace(spanish))
                        {
                            skipped++;
                            continue;
                        }

                        DataType data = directory.GetById(contentID);

                        if (data == null)
                        {
                            Debug.LogWarning(
                                $"Localization: SO not found. ID: {contentID}"
                            );

                            missing++;
                            continue;
                        }

                        if (!(data is ILocalizable localizable))
                        {
                            Debug.LogWarning(
                                $"Localization: {data.name} does not implement ILocalizable."
                            );

                            skipped++;
                            continue;
                        }

                        Undo.RecordObject(data, "Import Localization");

                        localizable.SetLocalizedText(
                            key,
                            "es",
                            spanish
                        );

                        EditorUtility.SetDirty(data);

                        imported++;
                    }
                }
            }

            EditorUtility.SetDirty(directory);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                "Import completed",
                $"Imported: {imported}\n" +
                $"Missing SOs: {missing}\n" +
                $"Skipped: {skipped}",
                "OK"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Localization import error:\n{e}");
        }
    }
}
