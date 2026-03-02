using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New TVNewRandom", menuName = "News/RandomNew")]
public class TVRandomNewType : ContentType, INewType, IReseteableScriptableObject, IPopUp
{
    [SerializeField][TextArea(minLines: 3, maxLines: 10)] string headline;
    [TextArea(minLines: 3, maxLines: 10)][SerializeField] string headlineTwoLines;
    [SerializeField][TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] int alertLevelIncrement;
    [SerializeField] int channel;
    [NonSerialized] bool wasStremed;


   /* private void OnEnable()
    {
        ScriptableObjectResetter.instance?.RegisterScriptableObject(this);
    }*/

    public void ResetScriptableObject()
    {
        wasStremed = false;
    }

    public int GetChannelNum()
    {
        return channel;
    }

    public string GetHeadline() { return headline; }

    public override string GetText() { return text; }
    public override string GetTextSecundary()
    {
        if (headline != "") return headline;
        else return headlineTwoLines;
    }

    public bool GetIfIsAEmergency() { return false; }

    public int GetIncreaseAlertLevel()
    {
        return alertLevelIncrement;
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

    public NewType GetNewType()
    {
        return NewType.RandomNews;
    }

    //POPUP implementation

    public string PopupText
    {
        get { return ""; }
    }

    public PopUpType PopUpType
    {
        get
        {
            return PopUpType.None;
        }
    }
}
