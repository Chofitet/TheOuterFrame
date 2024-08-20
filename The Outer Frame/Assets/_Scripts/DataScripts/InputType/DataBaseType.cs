using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BDEnter", menuName = "DB")]
public class DataBaseType : ScriptableObject
{
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string text;
    [SerializeField] Sprite image;
    [SerializeField] WordData AccessWord;
    [SerializeField] string phoneNum;
    [SerializeField] string age;
    [SerializeField] string location;
    [SerializeField] string born;
    [SerializeField] string occupation;
    [SerializeField] string found;
    [SerializeField] string status;
    [SerializeField] string government;
    [SerializeField] string populatoin;
    [SerializeField] string area;
    [SerializeField] string zipcode;
    [SerializeField] string areacode;
    [SerializeField] string classification;
    [SerializeField] string serial;
    [NonSerialized] bool isWordAccessFound;

    public WordData GetAccessWord() { return AccessWord; }
    public string GetText() { return text; }

    public Sprite GetImage() { return image; }

    public string GetPhoneNum() { return phoneNum; }

    public void SetisWordAccessFound() => isWordAccessFound = true;

    public bool GetisWordAccessFound() { return isWordAccessFound; }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }



}
