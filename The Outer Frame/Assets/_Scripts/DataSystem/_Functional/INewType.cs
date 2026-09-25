using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INewType
{
    NewType GetNewType();
    string GetHeadline();

    string GetHeadline2();

    string GetNewText();

    Sprite GetNewImag();

    bool GetIfIsAEmergency();

    int GetPriority();

    int GetIncreaseAlertLevel();

    int GetChannelNum();

    bool GetStateConditionalToAppear();
    int GetTimeToAppear();

    void SetStremedTime(TimeData time);
    int GetMinTransmitionTime();

    void SetWasStreamed();

    bool GetWasStreamed();

    void SetTimeStreamedEnds(TimeData time);

}
