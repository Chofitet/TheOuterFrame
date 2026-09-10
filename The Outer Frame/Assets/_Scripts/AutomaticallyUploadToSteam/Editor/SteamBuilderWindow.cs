using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Build.Reporting;
using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.UIElements;

public class SteamBuilderWindow : EditorWindow
{

    private TextField appIdField;
    private TextField unityBuildFolderField;
    private TextField contentRootField;
    private TextField buildOutputField;

    private PopupField<string> branchField;
    private TextField buildDescriptionField;

    public SteamBuilderConfig config;

    public string description;

    [MenuItem("Tools/Steam Builder")]
    public static void ShowWindow()
    {
        SteamBuilderWindow window = GetWindow<SteamBuilderWindow>();

        window.titleContent = new GUIContent("Steam Builder");

        // Tamaño inicial de la ventana
        window.minSize = new Vector2(500, 600);
        window.maxSize = new Vector2(700, 800);
    }


    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        // root.dataSource = this;


        // =====================================================
        // CONFIG HEADER
        // =====================================================

        VisualElement configHeader = new VisualElement();

        configHeader.style.flexDirection = FlexDirection.Row;
        configHeader.style.alignItems = Align.Center;
        configHeader.style.height = 30;
        configHeader.style.marginBottom = 10;


        Label configTitle = new Label("Config");

        configTitle.style.fontSize = 18;
        configTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
        configTitle.style.flexGrow = 1;


        Button openButton = new Button(OnOpenConfig)
        {
            text = "Open"
        };


        openButton.style.width = 50;


        configHeader.Add(configTitle);
        configHeader.Add(openButton);

        root.Add(configHeader);


        // App ID
        root.Add(CreateTextField(
            "App Id",
            out appIdField,
            false
        ));


        // Unity Build Folder
        root.Add(CreateFolderField(
     "Unity Build Folder",
     out unityBuildFolderField,
     OnSelectBuildFolder
        ));


        // Content Root
        root.Add(CreateFolderField(
            "Content Root",
            out contentRootField, OnSelectContentFolder
        ));


        // Build Output
        root.Add(CreateFolderField(
            "Build Output",
            out buildOutputField, OnSelectBuildOutput
        ));


        appIdField.value = config.appId;
        unityBuildFolderField.value = config.unityBuildFolder;
        contentRootField.value = config.contentRoot;
        buildOutputField.value = config.buildOutput;

        openButton.style.alignSelf = Align.FlexEnd;
        openButton.style.marginTop = 0;
        openButton.style.marginRight = 10;

        root.Add(openButton);

        //View depots

        SerializedObject serializedConfig = new SerializedObject(config);

        PropertyField steamDepotsPropertyField =
            new PropertyField(
                serializedConfig.FindProperty("depots")
            );

        root.Add(steamDepotsPropertyField);

        steamDepotsPropertyField.Bind(serializedConfig);


        // Spacer
        root.Add(new VisualElement
        {
            style =
            {
                height = 30
            }
        });


        // -------------------------------------------------
        // BUILD INFO
        // -------------------------------------------------

        Label buildInfoTitle = new Label("Build Info");
        buildInfoTitle.AddToClassList("section-title");

        root.Add(buildInfoTitle);


        branchField = new PopupField<string>(
            "Branch",
            config.branches,
            config.branchIndex
        );

        branchField.style.marginBottom = 8;

        branchField.RegisterValueChangedCallback(evt =>
        {
            config.branchIndex = branchField.index;
            EditorUtility.SetDirty(config);
        });

        root.Add(branchField);


        // Build Description
        buildDescriptionField = new TextField("Build Description")
        {
            multiline = true
        };

        buildDescriptionField.style.height = 100;
        buildDescriptionField.style.marginBottom = 30;

        buildDescriptionField.RegisterValueChangedCallback(evt =>
        {
            description = evt.newValue;
        });

        root.Add(buildDescriptionField);

        // -------------------------------------------------
        // WHAT WOULD YOU LIKE TO DO?
        // -------------------------------------------------

        Label actionTitle = new Label("What would you like to do?");

        actionTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        actionTitle.style.fontSize = 18;
        actionTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
        actionTitle.style.marginBottom = 10;

        root.Add(actionTitle);


        VisualElement buttonContainer = new VisualElement();

        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.justifyContent = Justify.Center;
        buttonContainer.style.paddingTop = 10;
        buttonContainer.style.paddingBottom = 10;


        Button buildButton = new Button(OnBuild)
        {
            text = "Build"
        };

        Button buildAndRunButton = new Button(OnBuildAndRun)
        {
            text = "Build and Run"
        };

        Button copyButton = new Button(OnCopy)
        {
            text = "Copy"
        };

        Button uploadButton = new Button(OnUpload)
        {
            text = "Upload"
        };

        Button buildAndUploadButton = new Button(OnBuildAndUpload)
        {
            text = "Build And Upload"
        };


        buttonContainer.Add(buildButton);
        buttonContainer.Add(buildAndRunButton);
        buttonContainer.Add(copyButton);
        buttonContainer.Add(uploadButton);
        buttonContainer.Add(buildAndUploadButton);


        root.Add(buttonContainer);
    }


    // =====================================================
    // CREACIÓN DE CAMPOS
    // =====================================================

    private VisualElement CreateTextField(
        string label,
        out TextField field,
        bool multiline)
    {
        VisualElement container = new VisualElement();

        container.style.flexDirection = FlexDirection.Row;
        container.style.marginBottom = 4;

        Label labelElement = new Label(label);

        labelElement.style.width = 150;
        labelElement.style.minWidth = 150;

        field = new TextField();

        field.style.flexGrow = 1;

        field.multiline = multiline;

        container.Add(labelElement);
        container.Add(field);

        return container;
    }


    private VisualElement CreateFolderField(
     string label,
     out TextField field, Action browseAction)
    {
        VisualElement container = new VisualElement();

        container.style.flexDirection = FlexDirection.Row;
        container.style.marginBottom = 4;

        Label labelElement = new Label(label);

        labelElement.style.width = 150;
        labelElement.style.minWidth = 150;

        TextField textField = new TextField();

        textField.style.flexGrow = 1;

        Button browseButton = new Button(browseAction);

        browseButton.text = "open";
        browseButton.style.width = 35;

        container.Add(labelElement);
        container.Add(textField);
        container.Add(browseButton);

        field = textField;

        return container;
    }


    // =====================================================
    // CALLBACKS
    // =====================================================

    private void OnSelectBuildFolder()
    {
        string path = EditorUtility.OpenFolderPanel(
            "Set Build Folder",
            "",
            ""
        );

        if (string.IsNullOrEmpty(path))
            return;

        path = Path.GetRelativePath(Application.dataPath, path);
        path = path.Replace('/', '\\');

        unityBuildFolderField.value = path;

        config.unityBuildFolder = path;

        EditorUtility.SetDirty(config);
    }

    private void OnSelectContentFolder()
    {
            string path = EditorUtility.OpenFolderPanel(
            "Set Content Root Folder",
            SteamBuilderWindowUITK.STEAM_BUILDER_PATH,
            ""
        );

            if (string.IsNullOrEmpty(path))
                return;

            path = Path.GetRelativePath(
                SteamBuilderWindowUITK.STEAM_BUILDER_PATH,
                path
            );

            path = path.Replace('/', '\\');

            contentRootField.value = path;

            config.contentRoot = path;

            EditorUtility.SetDirty(config);
    }

    private void OnSelectBuildOutput()
    {
        string path = EditorUtility.OpenFolderPanel(
        "Set Build Output Folder",
        SteamBuilderWindowUITK.STEAM_BUILDER_PATH,
        ""
    );

        if (string.IsNullOrEmpty(path))
            return;

        path = Path.GetRelativePath(
            SteamBuilderWindowUITK.STEAM_BUILDER_PATH,
            path
        );

        path = path.Replace('/', '\\');

        buildOutputField.value = path;

        config.buildOutput = path;

        EditorUtility.SetDirty(config);
    }

    private void OnOpenConfig()
    {
        EditorUtility.OpenPropertyEditor(config);
    }

    private void OnBuild()
    {
        if (Build())
        {
            string buildPath = Path.GetFullPath(
                config.unityBuildFolder,
                Application.dataPath
            );

            DirectoryHelper.OpenFolder(buildPath);
        }
    }

    private void OnBuildAndRun()
    {
        if (Build())
        {
            string buildPath = GetBuildPath();

            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = buildPath,
                    UseShellExecute = true
                }
            );
        }
    }

    private bool Build()
    {
        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray(),

            locationPathName = GetBuildPath(),

            target = EditorUserBuildSettings.activeBuildTarget,

            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        if(summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build succeeded");
            return true;
        }
        else if(summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log("Build failed");
        }
        return false;
    }

    private string GetBuildPath()
    {
        string folder = Path.GetFullPath(config.unityBuildFolder, Application.dataPath);

        if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
                return $"{folder}/TheOuterFrame.exe";
            case BuildTarget.StandaloneOSX:
                return $"{folder}/TheOuterFrame.app";
            case BuildTarget.Android:
                return $"{folder}/TheOuterFrame.apk";
            default:
                return $"{folder}/Build";
        }
    }
  
    private void OnCopy()
    {
        string buildPath = Path.GetFullPath(
        config.unityBuildFolder,
        Application.dataPath);

        string contentPath = Path.GetFullPath(
            config.contentRoot,
            SteamBuilderWindowUITK.STEAM_BUILDER_PATH
        );

        foreach (SteamBuilderDepot depot in config.depots)
        {
            string localPath = depot.localPath;

            // Remove the wildcard used by SteamCMD
            localPath = localPath.Replace("\\*", "");
            localPath = localPath.Replace("/*", "");

            string depotDestination = Path.Combine(
                contentPath,
                localPath
            );

            DirectoryHelper.CopyDirectory(
                buildPath,
                depotDestination
            );
        }

        DirectoryHelper.OpenFolder(contentPath);
    }


    private void OnUpload()
    {
        //Stop upload chance if description is empty
        if (String.IsNullOrEmpty(description))
        {
            if (!EditorUtility.DisplayDialog("empty Description", "Build description is empy, are you sure you want to upload?", "Yes", "No"))
            { return; };
        }

        string scriptFolder = Path.Combine(SteamBuilderWindowUITK.ENV_STEAM_SDK, "tools/ContentBuilder/scripts/");

        foreach (SteamBuilderDepot depot in config.depots)
        {
            string depotIdFilePath = string.Format("{0}depot_build_{1}.vdf", scriptFolder, depot.depotID);

            if (!File.Exists(depotIdFilePath))
            {
                File.Create(depotIdFilePath);
            }
            StreamWriter depotWriter = new StreamWriter(depotIdFilePath, false);

            string depotString = "\"DepotBuildConfig\"{" +
                "\"DepotID\" \"" + depot.depotID + "\"" +
                "\"ContentRoot\" \"" + depot.contentRoot + "\"" +
                "\"FileMapping\" {" +
                "\"LocalPath\" \"" + depot.localPath + "\"" +
                "\"DepotPath\" \"" + depot.depotPath + "\"" +
                 "\"recursive\" \"" + depot.recursive + "\"" +
                 "}" +
                "\"FileExclusion\" \"" + depot.fileExclusion + "\"" +
                "}";
            depotWriter.Write(depotString);
            depotWriter.Close();
        }

        string appIdFilePath = string.Format("{0}app_build_{1}.vdf", scriptFolder, config.appId);

        if (!File.Exists(appIdFilePath))
        {
            File.Create(appIdFilePath);
        }

        StreamWriter writer = new StreamWriter(appIdFilePath, false);
        string depotsLine = "{\n";
        foreach (SteamBuilderDepot depot in config.depots)
        {
            string depotString = "\"" + depot.depotID + "\" \"depot_build_" + depot.depotID + ".vdf\"";
            depotsLine += depotString + "\n";
        }
        depotsLine += "\n}";

        var branch = "";
        if (config.branches.Count != 0)
        {
            branch = config.branches[config.branchIndex];
        }
        string appIDLine = "\"appbuild\"" +
             "{\n" +
             "\"appid\" \"" + config.appId + "\"\n" +
             "\"desc\" \"" + description + "\"\n" +
             "\"buildoutput\" \"" + config.buildOutput + "\"\n" +
             "\"contentroot\" \"..\\content\"\n" +
             "\"setlive\" \"" + branch + "\"\n" +
             "\"depots\"" + depotsLine + "\n" +
             "}";
        writer.Write(appIDLine);
        writer.Close();

        string path = Path.Combine(SteamBuilderWindowUITK.ENV_STEAM_SDK, "tools\\ContentBuilder");
        StreamWriter runSteamCmdBatch = new StreamWriter(path + "/run_build.bat");

        runSteamCmdBatch.Write($"builder\\steamcmd.exe +login {SteamBuilderWindowUITK.ENV_STEAM_ID} {SteamBuilderWindowUITK.ENV_STEAM_PASSWORD} " +
        $"+run_app_build_http ..\\scripts\\app_build_{config.appId}.vdf");
        runSteamCmdBatch.Close();

        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("run_build.bat");
        info.WorkingDirectory = path;
        System.Diagnostics.Process.Start(info);
    }


    private void OnBuildAndUpload()
    {
        UnityEngine.Debug.Log("Build And Upload");

        if(Build())
        {
            OnCopy();
            OnUpload();
        }
    }
}

public class SteamBuilderWindowUITK : EditorWindow
{
    public static string ENV_STEAM_SDK
    {
        get
        {
            return Environment.GetEnvironmentVariable("STEAM_SDK", EnvironmentVariableTarget.User);
        }
    }
    public static string ENV_STEAM_ID
    {
        get
        {
            return Environment.GetEnvironmentVariable("STEAM_ID", EnvironmentVariableTarget.User);
        }
    }
    public static string ENV_STEAM_PASSWORD
    {
        get
        {
            return Environment.GetEnvironmentVariable("STEAM_PASSWORD", EnvironmentVariableTarget.User);
        }
    }

    public static string STEAM_BUILDER_PATH => Path.Combine(ENV_STEAM_SDK, "tools/ContentBuilder/");
}

public static class DirectoryHelper
{
    public static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = true)
    {
        if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

        //Create destination directory if it doesn´t exist
        Directory.CreateDirectory(destinationDir);

        //Copy files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite);
        }

        //Copy subdirectories recursiverly
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var destSubDir = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, destSubDir, overwrite);
        }
    }

    public static void OpenFolder(string path )
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }
}