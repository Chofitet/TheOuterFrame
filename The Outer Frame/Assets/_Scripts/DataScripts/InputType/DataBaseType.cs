using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BDEnter", menuName = "Input/BDEnter")]
public class DataBaseType : ScriptableObject
{
    [SerializeField] string text;
    [SerializeField] Sprite image;
    [SerializeField] string phoneNum;

    public string GetText() { return text; }

    public Sprite GetImage() { return image; }

    public string GetPhoneNum() { return phoneNum; }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }



}
