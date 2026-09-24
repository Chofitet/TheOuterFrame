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
    News,

    //board
    [AnalyticsInfo("Go To Board", "Board")]
    GoToBoard,
    [AnalyticsInfo("Photo Placed", "Board")]
    PhotoPlacedInBoard,
    [AnalyticsInfo("Idea Taked", "Board")]
    IdeaTakedInBoard,
    [AnalyticsInfo("Idea Sended", "Board")]
    IdeaSended,

    //PC
    [AnalyticsInfo("DB Word Searched", "PC")]
    DBWordSearched,
    [AnalyticsInfo("Hyper link Clicked", "PC")]
    HyperlinkClicked,
    //FindableWordInPC

    //WireOphone
    [AnalyticsInfo("Phone Number Finded", "WireOPhone")]
    PhoneNumberFinded,
    [AnalyticsInfo("REC Button Pushed", "WireOPhone")]
    RECButtonPushed,
    [AnalyticsInfo("Print Button Pushed", "WireOPhone")]
    PrintButtonPushed,

    //AP
    [AnalyticsInfo("Word Filled In AP", "AP")]
    WordFilledInAP,
    [AnalyticsInfo("Ap Sended", "AP")]
    ApSended

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