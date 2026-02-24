using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pre_ProccessFindableWords))]

public class ProccessFindableWordsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Pre_ProccessFindableWords script = (Pre_ProccessFindableWords)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Process Findable Words"))
        {
            script.ProcessInEditor();
        }
    }
}
