using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pre_ProccessFindableWords))]

public class ProccessFindableWordsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Pre_ProccessFindableWords script = (Pre_ProccessFindableWords)target;

        GUILayout.Space(15);
        EditorGUILayout.LabelField("Findable Processing", EditorStyles.boldLabel);

        GUILayout.Space(5);

        GUILayout.Space(10);

        if (GUILayout.Button("Process All Reports"))
        {
            script.ProcessAllReports();
        }

        if (GUILayout.Button("Process All Transcripts"))
        {
            script.ProcessAllTranscripts();
        }

        if (GUILayout.Button("Process All Database Entries"))
        {
            script.ProcessAllDatabaseEntries();
        }

        if(GUILayout.Button("Process All Pending Data"))
        {
            script.ProcessAllPendingData();
        }

        GUILayout.Space(10);

        GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);

        if (GUILayout.Button("PROCESS EVERYTHING"))
        {
            script.ProcessAllReports();
            script.ProcessAllTranscripts();
            script.ProcessAllDatabaseEntries();

            Debug.Log("All content processed.");
        }

        GUI.backgroundColor = Color.white;

    }

}
