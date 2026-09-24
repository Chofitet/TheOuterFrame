using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticMarkTrigger : MonoBehaviour
{
    [SerializeField] AnalyticsType analyticsType;
    List<AnalyticsEntry> MarksList = new List<AnalyticsEntry>();

    public void SetANewMaek(Component sender, object obj)
    {
        SetANewMark(TimeManager.timeManager.GetTime());
    }

    public void SetANewMark(TimeData time)
    {
        AnalyticsEntry entry = new AnalyticsEntry
        {
            ID = "no-id",
            CompletedTime = time,
            FinishedTime = time,
            WasDone = true,
            AnalyticType = analyticsType,
            VisualType = AnalyticsVisualType.Mark
        };

        MarksList.Add( entry);

        DatatService.instance.AddAnalyticMark(entry);
    }
}
