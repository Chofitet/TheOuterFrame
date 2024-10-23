using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindReferenceInScriptableObjects : EditorWindow
{
    [SerializeField] private Object objectToFind; // El objeto que quieres buscar, como una imagen
    private List<ScriptableObject> foundReferences = new List<ScriptableObject>(); // Lista de ScriptableObjects encontrados
    private Vector2 scrollPos; // Para manejar el scroll en la lista de resultados

    [MenuItem("Tools/Find References in ScriptableObjects")]
    public static void ShowWindow()
    {
        GetWindow<FindReferenceInScriptableObjects>("Find References");
    }

    private void OnGUI()
    {
        // Campo para seleccionar el objeto que quieres buscar (imagen, textura, etc.)
        objectToFind = EditorGUILayout.ObjectField("Object to Find", objectToFind, typeof(Object), false);

        if (GUILayout.Button("Find References"))
        {
            FindReferences();
        }

        // Si hay referencias encontradas, mostramos la lista
        if (foundReferences.Count > 0)
        {
            EditorGUILayout.LabelField("Found References:", EditorStyles.boldLabel);

            // Hacemos un scroll para la lista
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

            foreach (var reference in foundReferences)
            {
                EditorGUILayout.BeginHorizontal();

                // Mostrar el nombre del ScriptableObject con un botón que permite seleccionarlo
                if (GUILayout.Button(reference.name, GUILayout.Width(200)))
                {
                    // Selecciona el ScriptableObject en el Project
                    Selection.activeObject = reference;
                    EditorGUIUtility.PingObject(reference);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void FindReferences()
    {
        // Limpiamos la lista de resultados antes de cada búsqueda
        foundReferences.Clear();

        if (objectToFind == null)
        {
            Debug.LogError("Please assign an object to find.");
            return;
        }

        // Buscar todos los ScriptableObjects en el proyecto
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

            if (scriptableObject != null)
            {
                // Revisa los campos serializados de cada ScriptableObject
                var serializedObject = new SerializedObject(scriptableObject);
                var iterator = serializedObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    // Verifica si el campo contiene una referencia al objeto que estamos buscando
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (iterator.objectReferenceValue == objectToFind)
                        {
                            // Agregamos el ScriptableObject a la lista de referencias encontradas
                            foundReferences.Add(scriptableObject);
                        }
                    }
                }
            }
        }

        // Si no se encontraron referencias, mostramos un mensaje
        if (foundReferences.Count == 0)
        {
            Debug.Log("No references found.");
        }
    }
}

