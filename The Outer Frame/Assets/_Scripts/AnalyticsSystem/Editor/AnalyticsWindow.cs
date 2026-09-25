using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class AnalyticsWindow : EditorWindow
{
    private AnalyticsFile analyticsFile;
    [SerializeField] private DataDirectory directory;

    [MenuItem("Tools/Analytics Viewer")]
    public static void ShowWindow()
    {
        GetWindow<AnalyticsWindow>("Analytics");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("Import Analytics JSON", GUILayout.Height(30)))
        {
            ImportAnalytics();
        }

        EditorGUILayout.Space();

        if (analyticsFile == null)
        {
            EditorGUILayout.HelpBox(
                "Import an Analytics JSON file to visualize the run.",
                MessageType.Info
            );

            return;
        }

        DrawTimeline();
    }

    private void ImportAnalytics()
    {
        string analyticsDirectory = Path.Combine(
     Application.persistentDataPath,
     "Analytics"
        );

        string path = EditorUtility.OpenFilePanel(
            "Import Analytics JSON",
            analyticsDirectory,
            "json"
        );

        if (string.IsNullOrEmpty(path))
            return;

        string json = File.ReadAllText(path);

        analyticsFile = JsonUtility.FromJson<AnalyticsFile>(json);

        Repaint();
    }

    private DataType GetDataFromEntry(AnalyticsEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.ID))
            return null;

        return directory.GetById(entry.ID);
    }

    private string GetAssetName(DataType data)
    {
        if (data == null)
            return "Unknown";

        string path = AssetDatabase.GetAssetPath(data);

        return Path.GetFileNameWithoutExtension(path);
    }

    #region TimeLine
    private int GetMinutesFromStart(TimeData time)
    {
        int startMinutes = StartHour * 60 + StartMinute;
        int currentMinutes = time.Hour * 60 + time.Minute;

        return currentMinutes - startMinutes;
    }


    private const int StartHour = 4;
    private const int EndHour = 12;
    private const int StartMinute = 26;
    private const int EndMinute = 00;

    private const float TimelineWidth = 800f;
    private const float TimelineHeight = 80f;

    private const float LabelWidth = 150f;
    private const float RowHeight = 45f;

    private const float SubRowHeight = RowHeight / 2f;

    private void DrawTimeline()
    {
        EditorGUILayout.LabelField(
         $"Entries: {analyticsFile.Entries.Count}",
         EditorStyles.boldLabel
     );

        EditorGUILayout.Space();

        List<TimelineRow> rows = GetTimelineRows();

        Rect timelineRect = GUILayoutUtility.GetRect(
            1000f,
            RowHeight * (rows.Count + 1)
        );

        Rect timelineArea = new Rect(
            timelineRect.x + LabelWidth,
            timelineRect.y,
            timelineRect.width - LabelWidth,
            timelineRect.height
        );

        DrawTimeMarkers(timelineArea);

        DrawRows(timelineRect, timelineArea, rows);

        DrawBlockEntries(timelineArea);
        DrawMarkEntries(timelineArea);
    }

    private void DrawRows(
     Rect timelineRect,
     Rect timelineArea,
     List<TimelineRow> rows)
    {
        int rowIndex = 0;
        float currentY = timelineRect.y + RowHeight;

        foreach (TimelineRow row in rows)
        {
            if (row.IsGroup)
            {
                Rect groupRect = new Rect(
                    timelineRect.x,
                    currentY,
                    LabelWidth,
                    RowHeight
                );

                groupExpanded[row.Group] = EditorGUI.Foldout(
                  groupRect,
                  groupExpanded[row.Group],
                  row.Group,
                  true
              );

                EditorGUI.DrawRect(
                    new Rect(
                        timelineArea.x,
                        currentY,
                        timelineArea.width,
                        1
                    ),
                    Color.gray
                );

                currentY += RowHeight;
            }
            else
            {
                GUIStyle subRowStyle = new GUIStyle(EditorStyles.miniLabel);

                GUI.Label(
                    new Rect(
                        timelineRect.x + 20,
                        currentY,
                        LabelWidth - 20,
                        SubRowHeight
                    ),
                    GetAnalyticsTypeName(row.Type),
                    subRowStyle
                );

                EditorGUI.DrawRect(
                    new Rect(
                        timelineArea.x,
                        currentY,
                        timelineArea.width,
                        1
                    ),
                    Color.gray
                );

                currentY += SubRowHeight;
            }

            rowIndex++;
        }
    }

    private float GetRowY(AnalyticsType type, Rect timelineRect)
    {
        List<TimelineRow> rows = GetTimelineRows();

        float currentY = timelineRect.y + RowHeight;

        foreach (TimelineRow row in rows)
        {
            if (!row.IsGroup)
            {
                if (row.Type == type)
                    return currentY;

                currentY += SubRowHeight;
            }
            else
            {
                currentY += RowHeight;
            }
        }

        return -1;
    }

    private string GetAnalyticsTypeName(AnalyticsType type)
    {
        AnalyticsInfoAttribute info = GetAnalyticsInfo(type);

        return info != null
            ? info.Name
            : type.ToString();
    }

    private string GetAnalyticsGroup(AnalyticsType type)
    {
        AnalyticsInfoAttribute info = GetAnalyticsInfo(type);

        return info != null
            ? info.Group
            : "Other";
    }

    private Dictionary<string, bool> groupExpanded = new Dictionary<string, bool>();

    private class TimelineRow
    {
        public bool IsGroup;
        public string Group;
        public AnalyticsType Type;
    }

    private List<TimelineRow> GetTimelineRows()
    {
        List<TimelineRow> rows = new List<TimelineRow>();

        AnalyticsType[] types =
            (AnalyticsType[])System.Enum.GetValues(typeof(AnalyticsType));

        List<string> groups = new List<string>();

        foreach (AnalyticsType type in types)
        {
            string group = GetAnalyticsGroup(type);

            if (!groups.Contains(group))
                groups.Add(group);
        }

        foreach (string group in groups)
        {
            if (!groupExpanded.ContainsKey(group))
                groupExpanded[group] = true;

            rows.Add(new TimelineRow
            {
                IsGroup = true,
                Group = group
            });

            if (!groupExpanded[group])
                continue;

            foreach (AnalyticsType type in types)
            {
                if (GetAnalyticsGroup(type) != group)
                    continue;

                rows.Add(new TimelineRow
                {
                    IsGroup = false,
                    Group = group,
                    Type = type
                });
            }
        }

        return rows;
    }


    private AnalyticsInfoAttribute GetAnalyticsInfo(AnalyticsType type)
    {
        FieldInfo field = typeof(AnalyticsType).GetField(type.ToString());

        return field?
            .GetCustomAttribute<AnalyticsInfoAttribute>();
    }

    private int GetRowIndex(AnalyticsType type)
    {
        List<TimelineRow> rows = GetTimelineRows();

        for (int i = 0; i < rows.Count; i++)
        {
            if (!rows[i].IsGroup && rows[i].Type == type)
                return i;
        }

        return -1;
    }

    private void DrawTimeMarkers(Rect rect)
    {
        int startTotalMinutes = StartHour * 60 + StartMinute;
        int endTotalMinutes = EndHour * 60 + EndMinute;

        GUIStyle smallTimeStyle = new GUIStyle(EditorStyles.label);
        smallTimeStyle.fontSize = 8;

        int totalMinutes = endTotalMinutes - startTotalMinutes;

        // Primer marcador: 04:26
        {
            float x = rect.x;

            GUI.Label(
                new Rect(
                    x - 20,
                    rect.y,
                    50,
                    20
                ),
                $"{StartHour}:{StartMinute:00}"
            );

            EditorGUI.DrawRect(
                new Rect(
                    x,
                    rect.y + 20,
                    1,
                    16
                ),
                Color.gray
            );
        }

        // Marcadores cada 15 minutos,
        // empezando en el siguiente cuarto de hora.
        int firstQuarter = ((startTotalMinutes / 15) + 1) * 15;

        for (
            int currentMinutes = firstQuarter;
            currentMinutes <= endTotalMinutes;
            currentMinutes += 15)
        {
            float normalized =
                (currentMinutes - startTotalMinutes) /
                (float)totalMinutes;

            float x = rect.x + normalized * rect.width;

            int hour = currentMinutes / 60;
            int minutes = currentMinutes % 60;

            bool isHour = minutes == 0;

            GUI.Label(
                new Rect(
                    x - 12,
                    rect.y,
                    40,
                    20
                ),
                $"{hour}:{minutes:00}",
                isHour
                    ? EditorStyles.label
                    : smallTimeStyle
            );

            EditorGUI.DrawRect(
                new Rect(
                    x,
                    rect.y + 20,
                    1,
                    isHour ? 16 : 8
                ),
                Color.gray
            );
        }

        // Línea horizontal
        EditorGUI.DrawRect(
            new Rect(
                rect.x,
                rect.y + 20,
                rect.width,
                1
            ),
            Color.gray
        );
    }


    #endregion

    private void DrawBlockEntries(Rect rect)
    {
        foreach (AnalyticsEntry entry in analyticsFile.Entries)
        {
            if (entry.VisualType != AnalyticsVisualType.Block)
                continue;

            DataType data = directory.GetById(entry.ID);

            string assetName = data != null
                ? data.name
                : "Unknown";

            int minutes = GetMinutesFromStart(entry.CompletedTime);

            int totalMinutes = (EndHour - StartHour) * 60;

            float normalized =
                Mathf.Clamp01(minutes / (float)totalMinutes);

            float x = rect.x + normalized * rect.width;

            float rowY = GetRowY(entry.AnalyticType, rect);

            if (rowY < 0)
                continue;

            Rect entryRect = new Rect(
                x,
                rowY + 3,
                95,
                SubRowHeight - 6
            );

            EditorGUI.DrawRect(
                entryRect,
                new Color(0.25f, 0.5f, 0.8f)
            );

            float border = 2f;
            Color borderColor = Color.black;

            EditorGUI.DrawRect(
                new Rect(entryRect.x, entryRect.y, entryRect.width, border),
                borderColor
            );

            EditorGUI.DrawRect(
                new Rect(entryRect.x, entryRect.yMax - border, entryRect.width, border),
                borderColor
            );

            EditorGUI.DrawRect(
                new Rect(entryRect.x, entryRect.y, border, entryRect.height),
                borderColor
            );

            EditorGUI.DrawRect(
                new Rect(entryRect.xMax - border, entryRect.y, border, entryRect.height),
                borderColor
            );

            GUI.Label(
                entryRect,
                assetName
            );
        }
    }

    private void DrawMarkEntries(Rect rect)
    {
        foreach (AnalyticsEntry entry in analyticsFile.Entries)
        {
            if (entry.VisualType != AnalyticsVisualType.Mark)
                continue;

            int minutes = GetMinutesFromStart(entry.CompletedTime);

            int totalMinutes = (EndHour - StartHour) * 60;

            float normalized =
                Mathf.Clamp01(minutes / (float)totalMinutes);

            float x = rect.x + normalized * rect.width;

            float rowY = GetRowY(entry.AnalyticType, rect);

            if (rowY < 0)
                continue;

            Rect markRect = new Rect(
                x,
                rowY + 3,
                2,
                SubRowHeight - 6
            );

            EditorGUI.DrawRect(
                markRect,
                Color.red
            );
        }
    }
}
