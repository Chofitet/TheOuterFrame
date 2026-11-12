using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ClosedXML.Excel;

public static class LocalizationExporter
{
    private static readonly Dictionary<Type, string> SheetNames =
        new Dictionary<Type, string>()
        {
            { typeof(WordData), "WordsData" },
            { typeof(ReportType), "Reports" },
            { typeof(CallType), "Transcription Calls" },
            { typeof(DataBaseType), "DBs" },
            { typeof(TVNewType), "ReaciveNews" },
            { typeof(TVRandomNewType), "Random News" },
            { typeof(TVScheduledNewType), "Scheduled News" },
             { typeof(InGameText), "InGameText" },
            { typeof(StateEnum), "Actions" }
        };

    [MenuItem("Tools/Localization/Export Excel")]
    public static void Export()
    {
        string path = EditorUtility.SaveFilePanel(
            "Export Localization",
            "",
            "TOF_Localization.xlsx",
            "xlsx"
        );

        if (string.IsNullOrEmpty(path))
            return;

        using (var workbook = new XLWorkbook())
        {
            HashSet<string> uniqueTexts = new HashSet<string>();

            int totalWords = 0;
            int uniqueWords = 0;

            Dictionary<string, IXLWorksheet> worksheets =
                new Dictionary<string, IXLWorksheet>();

            Dictionary<string, int> nextRows =
                new Dictionary<string, int>();

            string[] assets = AssetDatabase.FindAssets("t:ScriptableObject");

            int exportedEntries = 0;
            int skippedAssets = 0;

            foreach (string guid in assets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                ScriptableObject asset =
                    AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                if (!(asset is DataType data) ||
                    !(asset is ILocalizable localizable))
                    continue;

                Type assetType = asset.GetType();

                if (!SheetNames.TryGetValue(assetType, out string sheetName))
                    continue;

                Guid contentID;

                try
                {
                    contentID = data.ID;
                }
                catch (FormatException)
                {
                    Debug.LogWarning($"Invalid ID: {asset.name}");
                    skippedAssets++;
                    continue;
                }

                if (contentID == Guid.Empty)
                {
                    Debug.LogWarning($"Asset without ID: {asset.name}");
                    skippedAssets++;
                    continue;
                }

                foreach (LocalizationEntry entry in localizable.GetLocalizationEntries())
                {
                    if (string.IsNullOrWhiteSpace(entry.English))
                        continue;

                    // Create the sheet only when it has something to export.
                    if (!worksheets.ContainsKey(sheetName))
                    {
                        IXLWorksheet sheet = workbook.Worksheets.Add(sheetName);

                        sheet.Cell(1, 1).Value = "ContentID";
                        sheet.Cell(1, 2).Value = "Words";
                        sheet.Cell(1, 3).Value = "Unique Words";
                        sheet.Cell(1, 4).Value = "FileName";
                        sheet.Cell(1, 5).Value = "Key";
                        sheet.Cell(1, 6).Value = "English";
                        sheet.Cell(1, 7).Value = "Spanish";

                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.SheetView.FreezeRows(1);

                        sheet.Column(1).Width = 38;
                        sheet.Column(2).Width = 25;
                        sheet.Column(3).Width = 25;
                        sheet.Column(4).Width = 80;
                        sheet.Column(4).Style.Alignment.WrapText = true;
                        sheet.Column(5).Width = 12;

                        worksheets.Add(sheetName, sheet);
                        nextRows.Add(sheetName, 2);
                    }

                    IXLWorksheet worksheet = worksheets[sheetName];
                    int row = nextRows[sheetName];

                    worksheet.Cell(row, 1).Value = contentID.ToString();

                    int wordCount = CountWords(entry.English);
                    totalWords += wordCount;

                    // Normalizamos saltos de línea para evitar diferencias innecesarias.
                    string normalizedText = entry.English.Trim()
                        .Replace("\r\n", "\n")
                        .Replace("\r", "\n");

                    int uniqueWordCount = 0;

                    if (uniqueTexts.Add(normalizedText))
                    {
                        uniqueWordCount = wordCount;
                        uniqueWords += wordCount;
                    }

                    worksheet.Cell(row, 2).Value = wordCount;
                    worksheet.Cell(row, 3).Value = uniqueWordCount;


                    worksheet.Cell(row, 4).Value = asset.name;
                    worksheet.Cell(row, 5).Value = entry.Key;
                    worksheet.Cell(row, 6).Value = entry.English;


                    nextRows[sheetName] = row + 1;
                    exportedEntries++;
                }
            }

            if (worksheets.Count == 0)
            {
                Debug.LogWarning("No localization entries found.");
                return;
            }

            workbook.SaveAs(path);

            Debug.Log(
                $"Localization exported! Entries: {exportedEntries}. " +
                $"Skipped assets: {skippedAssets}."
            );
        }
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(
            new char[] { ' ', '\t', '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries
        ).Length;
    }

    private static int FindColumn(IXLWorksheet worksheet, string header)
    {
        var firstRow = worksheet.FirstRowUsed();

        if (firstRow == null)
            return 0;

        foreach (var cell in firstRow.CellsUsed())
        {
            if (string.Equals(
                cell.GetString().Trim(),
                header,
                StringComparison.OrdinalIgnoreCase))
            {
                return cell.Address.ColumnNumber;
            }
        }

        return 0;
    }

    private static string EscapeForLog(string value)
    {
        if (value == null)
            return "null";

        return value
            .Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");
    }
}

