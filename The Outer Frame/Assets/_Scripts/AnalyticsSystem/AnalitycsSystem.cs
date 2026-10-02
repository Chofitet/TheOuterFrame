using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalitycsSystem : MonoBehaviour
{
    [SerializeField] GameEvent OnExportAnalytics;

    private void OnApplicationQuit()
    {
        ExportAnalytics(this, "Forced Quit Game");
    }

    public void ExportAnalytics(Component sender,object obj)
    {
        OnExportAnalytics?.Invoke(this, (string) obj);
    }
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
    [AnalyticsInfo("Went To TV", "TV")]
    WentToTV = 21,

    //board
    [AnalyticsInfo("Went To Board", "Board")]
    WentToBoard = 1,
    [AnalyticsInfo("Photo Placed", "Board")]
    PhotoPlaced = 2,
    [AnalyticsInfo("Idea Picked Up", "Board")]
    IdeaPickedUp = 3,
    [AnalyticsInfo("Idea Written", "Board")]
    IdeaWritten = 4,
    [AnalyticsInfo("Idea Sent", "Board")] //comprobar
    IdeaSent = 13,

    //PC
    [AnalyticsInfo("DB Word Searched", "PC")]
    DBWordSearched = 5,
    [AnalyticsInfo("Empty DB Serched", "PC")]
    EmptyDBSerched = 13,
    [AnalyticsInfo("Hiperlink Clicked", "PC")]
    HiperlinkClicked = 6,
    [AnalyticsInfo("Log Clicked", "PC")] // comprobar
    LogClicked = 14,
    [AnalyticsInfo("Subject Files Clicked", "PC")] // comprobar
    SubjectFilesClicked = 27,
    [AnalyticsInfo("Back from Log", "PC")] // comprobar
    BackFromLog = 15,

    //WireOphone
    [AnalyticsInfo("Phone Number Found", "WireOPhone")] // No implemented
    PhoneNumberFound = 7,
    [AnalyticsInfo("Phone Number Entered", "WireOPhone")]
    PhoneNumberEntered = 18,
    [AnalyticsInfo("Rec Pressed", "WireOPhone")]
    RecPressed = 8,
    [AnalyticsInfo("Print Pressed", "WireOPhone")] // comprobar
    PrintPressed = 9,
    [AnalyticsInfo("Cancel Pressed", "WireOPhone")] // comprobar
    CancelPressed = 17,
    [AnalyticsInfo("Wiretapping", "WireOPhone")] // comprobar
    Wiretapping = 19,

    //AP
    [AnalyticsInfo("Word Filled In AP", "AP")]
    WordFilledInAP = 10,
    [AnalyticsInfo("AP Sent", "AP")]
    ApSended = 11,

    //Time 
    [AnalyticsInfo("Time Accelerated", "Time")]
    TimeAccelerated = 12,
    [AnalyticsInfo("Time Back To Normal", "Time")] // No implemented // differents input types
    TimeBackToNormal = 20,

    //Progressor
    [AnalyticsInfo("Print From Desk View", "Progressor")] // comprobar
    PrintFromDeskView = 22,
    [AnalyticsInfo("Print From Progressor View", "Progressor")] // comprobar
    PrintFromProgressorView = 23,
    [AnalyticsInfo("Aborted an AP", "Progressor")] // comprobar
    AbortedAnAP = 24,
    [AnalyticsInfo("Cant Send, No agents", "Progressor")] // No implemented
    CantSendNoAgents = 25,
    [AnalyticsInfo("Cant Send, Printer full", "Progressor")] // No implemented
    CantSendPrinterFull = 26,

    //Findables
    [AnalyticsInfo("Found on Briefing", "Findable Words")] // comprobar
    FindableWordGeneric = 16,
    [AnalyticsInfo("Found on Report", "Findable Words")] // comprobar
    FindableWordInReport = 27,
    [AnalyticsInfo("Found on PC", "Findable Words")] // comprobar
    FindableWordInPC = 28,
    [AnalyticsInfo("Found on TV", "Findable Words")] // comprobar
    FindableWordInTV = 29,
    [AnalyticsInfo("Found on Transcript", "Findable Words")] // comprobar
    FindableWordInTranscript = 30


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