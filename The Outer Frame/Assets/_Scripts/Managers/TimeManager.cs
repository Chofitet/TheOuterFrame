using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChange;
    public static Action OnHourChange;
    public static Action OnDayChange;
    public float TimeVariation = 50;
    bool isTimeAccelerated;

    public static TimeManager timeManager { get; private set; }

    private void Awake()
    {

        if (timeManager != null && timeManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            timeManager = this;
        }

    }

    int Day;

    int Hour;

    int Minute;

    public int GetActualMinute()
    {
        return Minute;
    }

    public int GetActualHour()
    {
        return Hour;
    }

    public int GetActualDay()
    {
        return Day;
    }

    public TimeData GetTime()
    {
        return new TimeData(Day, Hour, Minute);
    }

    int seconds = 1;
    private float AlternativeTime;

    void Start()
    {
        Day = 00;
        Minute = 00;
        Hour = 00;
    }

    void Update()
    {
        AlternativeTime += Time.deltaTime * TimeVariation;
        if (AlternativeTime >= 60 * seconds)
        {
            Minute++;
            OnMinuteChange?.Invoke();
            if (Minute >= 60)
            {
                Hour++;
                Minute = 0;
                OnHourChange?.Invoke();
            }
            seconds++;
        }
    }

    private void OnEnable()
    {
        OnHourChange += MakeDayChange;
    }
    private void OnDisable()
    {
        OnHourChange -= MakeDayChange;
    }

    void MakeDayChange()
    {
        if (Hour == 24)
        {
            Day++;
            Hour = 0;
            OnDayChange?.Invoke();
        }
    }

    public void SpeedUpTime()
    {
        if (!isTimeAccelerated)
        {
            TimeVariation = 900f;
            isTimeAccelerated = true;
        }
        else { NormalizeTime(); }
    }

    public void NormalizeTime()
    {
        TimeVariation = 30;
        isTimeAccelerated = false;
    }

    public void PauseTime()
    {
        TimeVariation = 0;
    }
}

public struct TimeData
{
    public int Day;
    public int Hour;
    public int Minute;

    public TimeData(int day, int hour, int minute)
    {
        Day = day;
        Hour = hour;
        Minute = minute;
    }

    public override string ToString()
    {
        return $"Day: {Day}, Hour: {Hour}, Minute: {Minute}";
    }
}


