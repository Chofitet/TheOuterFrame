using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class Pre_ProccessFindableWords : MonoBehaviour
{
    [SerializeField] ContentType contentTypeTest;
    [SerializeField] FindableWordsManager findableWordsManager;
    [SerializeField] WordData Irrelevant;
    [SerializeField] DataDirectory directory;

    [Header("Report Type")]
    [SerializeField] TMP_Text reportField;

    [Header("Transcription Type")]
    [SerializeField] TMP_Text transcriptionField;

    [Header("DB Type")]
    [SerializeField] TMP_Text DBField;

    [Header("TV Type")] // Por ahora me voy a ahorrar hacer el pre-proccess de las noticias, son livianas y a su vez son un lio
    [SerializeField] TMP_Text TVTitleField;
    [SerializeField] TMP_Text TV2LinesTitleField;
    [SerializeField] TMP_Text TVTextField;

    private void ProcessContent(ContentType content, TMP_Text tmpField)
    {
        List<FindableWordData> combined = new();

        void Process(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            tmpField.text = text;
            tmpField.ForceMeshUpdate();

            var result = ProccessFindableWord.SearchForFindableWord(tmpField, Irrelevant);

            if (result != null && result.Count > 0)
                combined.AddRange(result);
        }

        Process(content.GetText());
        Process(content.GetTextSecundary());

        // eliminar duplicados por ID (mucho mejor que por nombre)
       /*- combined = combined
            .GroupBy(x => x.GetName())
            .Select(g => g.First())
            .ToList();*/

        content.SetFindableWords(combined);
        EditorUtility.SetDirty(content);
    }

    public void ProcessAllReports()
    {
        var reports = directory.GetAllReportTypes();

        for (int i = 0; i < reports.Count; i++)
        {
            var report = reports[i];

            EditorUtility.DisplayProgressBar(
                "Processing Reports",
                report.name,
                (float)i / reports.Count
            );

            ProcessContent(report, reportField);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();

        Debug.Log("Reports processed.");
    }

    public void ProcessAllTranscripts()
    {
        var calls = directory.GetAllTranscriptType();

        for (int i = 0; i < calls.Count; i++)
        {
            var call = calls[i];

            EditorUtility.DisplayProgressBar(
                "Processing Transcripts",
                call.name,
                (float)i / calls.Count
            );

            ProcessContent(call, transcriptionField);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();

        Debug.Log("Transcripts processed.");
    }

    public void ProcessAllDatabaseEntries()
    {
        var dbEntries = directory.GetAllDBType();

        for (int i = 0; i < dbEntries.Count; i++)
        {
            var db = dbEntries[i];

            EditorUtility.DisplayProgressBar(
                "Processing Database",
                db.name,
                (float)i / dbEntries.Count
            );

            ProcessContent(db, DBField);
            ProccessHyperlink(db,DBField);
            ProccessRedactedBlock(db, DBField);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();

        Debug.Log("Database entries processed.");
    }

    void ProccessHyperlink(DataBaseType content, TMP_Text tmpField)
    {
        var result = new List<FindableWordData>();

        void Process(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            tmpField.text = text;
            tmpField.ForceMeshUpdate();

            result = ProccessHyperLinks.SearchForHyperLinkWord(tmpField, Irrelevant);

        }

        Process(content.GetText());

        content.SetHyperLinks(result);
        EditorUtility.SetDirty(content);
    }

    void ProccessRedactedBlock(DataBaseType content, TMP_Text tmpField)
    {
        var result = new List<RedactedBlockData>();

        void Process(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            tmpField.text = text;
            tmpField.ForceMeshUpdate();

            result = ProcessRedactedBlock.SearchForRedactedBlocks(tmpField, false);

        }

        Process(content.GetText());

        content.SetRedactedBlocks(result);
        EditorUtility.SetDirty(content);
    }

}
