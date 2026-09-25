using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalitycsSystem : MonoBehaviour
{

}

[Serializable]
public class AnalyticsEntry
{
    public string ID;
    public TimeData CompletedTime;
    public TimeData FinishedTime;
    public bool WasDone;
    public AnalyticsType AnalyticType;
    public AnalyticsVisualType VisualType;
}

[System.Serializable]
public enum AnalyticsType
{
    // TV
    [AnalyticsInfo("News", "TV")]
    News = 0,

    //board
    [AnalyticsInfo("Go To Board", "Board")]
    GoToBoard = 1,
    [AnalyticsInfo("Photo Placed", "Board")]
    PhotoPlacedInBoard = 2,
    [AnalyticsInfo("Idea Taked", "Board")]
    IdeaTakedInBoard = 3,
    [AnalyticsInfo("Idea Sended", "Board")]
    IdeaSended = 4,

    //PC
    [AnalyticsInfo("DB Word Searched", "PC")]
    DBWordSearched = 5,
    [AnalyticsInfo("Empty DB Serched", "PC")]
    EmptyDBSerched = 13,
    [AnalyticsInfo("Hyper link Clicked", "PC")]
    HyperlinkClicked = 6,
    
    //FindableWordInPC

    //WireOphone
    [AnalyticsInfo("Phone Number Finded", "WireOPhone")]
    PhoneNumberFinded =7,
    [AnalyticsInfo("REC Button Pushed", "WireOPhone")]
    RECButtonPushed = 8,
    [AnalyticsInfo("Print Button Pushed", "WireOPhone")]
    PrintButtonPushed = 9,

    //AP
    [AnalyticsInfo("Word Filled In AP", "AP")]
    WordFilledInAP = 10,
    [AnalyticsInfo("Ap Sended", "AP")]
    ApSended = 11,

    //Time 
    [AnalyticsInfo("Time Accelerated", "Time")]
    TimeAccelerated = 12
}

public class AnalyticsInfoAttribute : Attribute
{
    public string Name { get; }
    public string Group { get; }

    public AnalyticsInfoAttribute(string name, string group)
    {
        Name = name;
        Group = group;
    }
}

public enum AnalyticsVisualType
{
    Block,
    Mark
}

public interface IAnalytics
{
    AnalyticsEntry GetAnalyticsData();
}