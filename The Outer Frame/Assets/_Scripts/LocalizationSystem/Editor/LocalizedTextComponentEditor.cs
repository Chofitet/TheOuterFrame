using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public static class LocalizedTextComponentEditor
{
    private const string folder =
         "Assets/ScriptableObjects/InGameTexts";

    [MenuItem("Tools/Localization/Synchronize InGameTexts")]
    public static void Synchronize()
    {
        EnsureFolder();

        SynchronizePrefabs();
        SynchronizeScenes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("InGameText Synchronization Complete!");
    }

    // =====================================
    // PREFABS
    // =====================================

    private static void SynchronizePrefabs()
    {
        string[] guids = AssetDatabase.FindAssets(
            "t:Prefab",
            new[] { "Assets" }
        );

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject root = PrefabUtility.LoadPrefabContents(path);

            try
            {
                bool changed = false;

                LocalizedTextComponent[] components =
                    root.GetComponentsInChildren<LocalizedTextComponent>(true);

                foreach (LocalizedTextComponent component in components)
                {
                    if (ProcessComponent(component))
                        changed = true;
                }

                if (changed)
                    PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }

    // =====================================
    // SCENES
    // =====================================

    private static void SynchronizeScenes()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (!scene.IsValid() || !scene.isLoaded)
            return;

        bool changed = false;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            LocalizedTextComponent[] components =
                root.GetComponentsInChildren<LocalizedTextComponent>(true);

            foreach (LocalizedTextComponent component in components)
            {
               

                if (ProcessComponent(component))
                    changed = true;
            }
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    // =====================================
    // PROCESS COMPONENT
    // =====================================

    private static bool ProcessComponent(
        LocalizedTextComponent component)
    {
        if (component == null || component.TextField == null)
            return false;

        TMP_Text tmp = component.TextField;

        // Si el texto está vacío, no hacemos nada con este componente.
        if (string.IsNullOrWhiteSpace(tmp.text))
            return false;

        InGameText content = component.Content;

        // Si todavía no tiene un InGameText, lo creamos.
        if (content == null)
        {
            content = CreateContent(tmp);

            component.AssignContent(content);
            EditorUtility.SetDirty(component);

            // Guardar el cambio si el componente pertenece a una instancia de prefab.
            if (PrefabUtility.IsPartOfPrefabInstance(component))
                PrefabUtility.RecordPrefabInstancePropertyModifications(component);

            return true;
        }

        // Si ya tiene un InGameText, comparamos y actualizamos si cambió.
        if (content.GetText() != tmp.text)
        {
            content.SetText(tmp.text);
            EditorUtility.SetDirty(content);

            return true;
        }

        return false;
    }

    private static InGameText CreateContent(TMP_Text tmp)
    {
        // Generamos el nombre y la ruta del asset.
        string fileName = GetAssetName(tmp.text);

        string path = AssetDatabase.GenerateUniqueAssetPath(
            $"{folder}/{fileName}.asset"
        );

        // Creamos el ScriptableObject.
        InGameText content = ScriptableObject.CreateInstance<InGameText>();
        content.SetText(tmp.text);

        // Guardamos el asset y lo registramos en el directorio.
        AssetDatabase.CreateAsset(content, path);
        DataServiceEditor.Register(content);

        EditorUtility.SetDirty(content);

        return content;
    }

    // =====================================
    // ASSET NAME
    // =====================================

    private static string GetAssetName(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "LocalizationText_Empty";

        string name = text.Trim()
            .Replace("\n", "_")
            .Replace("\r", "_")
            .Replace("\t", "_")
            .Replace(" ", "_");

        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        return $"LocalizationText_{name}";
    }

    // =====================================
    // FOLDER
    // =====================================

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder(
                "Assets",
                "ScriptableObjects"
            );
        }

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder(
                "Assets/ScriptableObjects",
                "InGameTexts"
            );
        }
    }
}
