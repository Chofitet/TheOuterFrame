
using UnityEditor;
using UnityEngine;

public class DictionaryEditorWindow : EditorWindow
{
    [MenuItem("Tools/Data Directory")]
    public static void ShowWindow()
    {
        GetWindow<DictionaryEditorWindow>("Data Directory");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        if (GUILayout.Button("Rebuild Directory", GUILayout.Height(40)))
        {
            DataServiceEditor.RebuildAllDirectory();
        }
    }
}
