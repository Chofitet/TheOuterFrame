using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public static class LocalizedTextComponentEditor
{
    private const string folder =
       "Assets/ScriptableObjects/InGameTexts";

    [MenuItem("Tools/Localization/Synchronize InGameTexts")]
    public static void Synchronize()
    {
        EnsureFolder();

        int created = 0;
        int updated = 0;
        int unchanged = 0;

        SynchronizePrefabs(ref created, ref updated, ref unchanged);
        SynchronizeScenes(ref created, ref updated, ref unchanged);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"InGameText Synchronization Complete!\n" +
            $"Created: {created}\n" +
            $"Updated: {updated}\n" +
            $"Unchanged: {unchanged}"
        );
    }

    // =====================================
    // PREFABS
    // =====================================

    private static void SynchronizePrefabs(
        ref int created,
        ref int updated,
        ref int unchanged)
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
                bool changed = ProcessGameObject(
                    root, ref created, ref updated, ref unchanged
                );

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

    private static void SynchronizeScenes(
        ref int created,
        ref int updated,
        ref int unchanged)
    {
        Scene scene = SceneManager.GetActiveScene();

        if (!scene.IsValid() || !scene.isLoaded)
            return;

        bool changed = false;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (ProcessGameObject(
                root, ref created, ref updated, ref unchanged))
            {
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
    // PROCESS GAME OBJECT
    // =====================================

    private static bool ProcessGameObject(
        GameObject root,
        ref int created,
        ref int updated,
        ref int unchanged)
    {
        bool changed = false;

        // Single texts
        LocalizedTextComponent[] singleComponents =
            root.GetComponentsInChildren<LocalizedTextComponent>(true);

        foreach (var component in singleComponents)
        {
            if (ProcessSingleText(component, ref created, ref updated, ref unchanged))
                changed = true;
        }

        // Lists of texts
        LocalizedTextComponentList[] listComponents =
            root.GetComponentsInChildren<LocalizedTextComponentList>(true);

        foreach (var component in listComponents)
        {
            if (ProcessTextList(component, ref created, ref updated, ref unchanged))
                changed = true;
        }

        return changed;
    }

    // =====================================
    // SINGLE TEXT
    // =====================================

    private static bool ProcessSingleText(
        LocalizedTextComponent component,
        ref int created,
        ref int updated,
        ref int unchanged)
    {
        if (component == null || component.TextField == null)
            return false;

        TMP_Text tmp = component.TextField;

        if (string.IsNullOrWhiteSpace(tmp.text))
            return false;

        InGameText content;

        bool changed = GetOrCreateContent(
            tmp.text,
            component.Content,
            component.GetAssetName(),
            out content
        );

        if (component.Content != content)
        {
            component.AssignContent(content);
            MarkComponentDirty(component);
            changed = true;
        }

        CountResult(changed, component.Content == content,
            ref created, ref updated, ref unchanged);

        return changed;
    }

    // =====================================
    // TEXT LIST
    // =====================================

    private static bool ProcessTextList(
        LocalizedTextComponentList component,
        ref int created,
        ref int updated,
        ref int unchanged)
    {
        if (component == null || component.Reference == null)
            return false;

        List<string> texts = component.Reference.GetHardcodedTexts();

        bool changed = false;

        for (int i = 0; i < texts.Count; i++)
        {
            string text = texts[i];

            if (string.IsNullOrWhiteSpace(text))
                continue;

            InGameText existing = i < component.Contents.Count
                ? component.Contents[i]
                : null;

            InGameText content;

            bool contentChanged = GetOrCreateContent(
                text,
                existing,
                "",
                out content
            );

            if (existing != content)
            {
                component.AssignContent(i, content);
                MarkComponentDirty(component);
                changed = true;
            }

            if (contentChanged)
                changed = true;

            CountResult(contentChanged, existing == null,
                ref created, ref updated, ref unchanged);
        }

        return changed;
    }

    // =====================================
    // SHARED CONTENT CREATION / UPDATE
    // =====================================

    private static bool GetOrCreateContent(
        string text,
        InGameText existing,
        string assetName,
        out InGameText content)
    {
        content = existing;

        if (existing == null)
        {
            content = CreateContent(text, assetName);
            return true;
        }

        if (existing.GetText() != text)
        {
            existing.SetText(text);
            EditorUtility.SetDirty(existing);
            return true;
        }

        return false;
    }

    private static InGameText CreateContent(
        string text,
        string assetName)
    {
        string fileName = GetAssetName(text, assetName);

        string path = AssetDatabase.GenerateUniqueAssetPath(
            $"{folder}/{fileName}.asset"
        );

        InGameText content =
            ScriptableObject.CreateInstance<InGameText>();

        content.SetText(text);

        AssetDatabase.CreateAsset(content, path);
        DataServiceEditor.Register(content);

        EditorUtility.SetDirty(content);

        return content;
    }

    // =====================================
    // HELPERS
    // =====================================

    private static void MarkComponentDirty(Component component)
    {
        EditorUtility.SetDirty(component);

        if (PrefabUtility.IsPartOfPrefabInstance(component))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(
                component
            );
        }
    }

    private static void CountResult(
        bool changed,
        bool wasCreated,
        ref int created,
        ref int updated,
        ref int unchanged)
    {
        if (!changed)
            unchanged++;
        else if (wasCreated)
            created++;
        else
            updated++;
    }

    // =====================================
    // ASSET NAME
    // =====================================

    private static string GetAssetName(
        string text,
        string assetName)
    {
        if (!string.IsNullOrEmpty(assetName))
            return $"LocalizationText_{assetName}";

        if (string.IsNullOrWhiteSpace(text))
            return "LocalizationText_Empty";

        string name = string.Join("_", text.Trim()
            .Split(
                new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries
            )
            .Take(10));

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
