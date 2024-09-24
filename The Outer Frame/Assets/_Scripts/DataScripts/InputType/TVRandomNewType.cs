using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New TVNewRandom", menuName = "News/RandomNew")]
public class TVRandomNewType : ScriptableObject, INewType
{
    [SerializeField][TextArea(minLines: 3, maxLines: 10)] string headline;
    [TextArea(minLines: 3, maxLines: 10)] [SerializeField] string headlineTwoLines;
    [SerializeField][TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] int channel;
    [NonSerialized] bool wasStremed;

    public int GetChannelNum()
    {
        return channel;
    }

    public string GetHeadline(){return headline;}

    public bool GetIfIsAEmergency() { return false; }

    public int GetIncreaseAlertLevel()
    {
        return 0;
    }

    public int GetMinTransmitionTime()
    {
        return 0;
    }

    public Sprite GetNewImag() { return image; }

    public string GetNewText()
    {
        return text;
    }

    public bool GetStateConditionalToAppear()
    {
        return true;
    }

    public int GetTimeToAppear()
    {
        return 0;
    }

    public int GetPriority()
    {
        return 100;
    }
    public void SetWasStreamed()
    {
        wasStremed = true;
    }

    public bool GetWasStreamed()
    {
        return wasStremed;
    }

    public string GetHeadline2()
    {
        return headlineTwoLines;
    }
}
