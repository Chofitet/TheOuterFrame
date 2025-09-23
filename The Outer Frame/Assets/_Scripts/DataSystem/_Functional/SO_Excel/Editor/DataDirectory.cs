using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataDirectory : EditorWindow
{
    private DataBaseMediator mediator;
    private DataBaseType selectedSO;

    [MenuItem("Tools/DataBase Window")]
    public static void OpenWindow()
    {
        GetWindow<DataDirectory>("DataBase Mediator");
    }

    private void OnEnable()
    {
        // Crear instancia interna del mediator
        if (mediator == null)
            mediator = new DataBaseMediator();
    }


    private void OnGUI()
    {
        GUILayout.Label("DataBase Mediator Tool", EditorStyles.boldLabel);

     
        // Seleccionar ScriptableObject
        selectedSO = (DataBaseType)EditorGUILayout.ObjectField("DataBase SO", selectedSO, typeof(DataBaseType), false);

        GUILayout.Space(10);

        if (mediator != null && selectedSO != null)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Export to CSV"))
            {
                mediator.SetDataOnCVS(0, selectedSO);
                Debug.Log("Exported to CSV!");
            }

            if (GUILayout.Button("Import from CSV"))
            {
                mediator.SetDataOnSO(0, selectedSO);
                Debug.Log("Imported from CSV!");
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Assign a mediator and a DataBase SO to use the buttons.");
        }
    }
}
