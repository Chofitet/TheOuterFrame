using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WordData))]
public class AssingStatesToWordsDebug : Editor
{
    private StateEnum newState;

    public override void OnInspectorGUI()
    {
        // Llamar a la implementación base (dibuja el inspector predeterminado)
        base.OnInspectorGUI();

        // Obtener la referencia al objeto que se está inspeccionando
        WordData wordData = (WordData)target;

        // Mostrar un campo para asignar el nuevo estado
        newState = (StateEnum)EditorGUILayout.ObjectField("state", newState, typeof(StateEnum), false);

        // Botón para agregar el estado al historial de estados
        if (GUILayout.Button("Add State to history's word"))
        {
            if (newState != null)
            {
                wordData.AddStateInHistory(newState);
                wordData.CheckStateSeen(newState);
                Debug.Log($"Estado {newState.name} agregado al historial.");
            }
        }

        // Redibujar el inspector cuando se presiona el botón
        if (GUI.changed)
        {
            EditorUtility.SetDirty(wordData);
        }
    }
}

