using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static Action OnSecondsChange;
    public static Action OnMinuteChange;
    public static Action OnHourChange;
    public static Action OnDayChange;
    public static Action OnNewsChange;
    [SerializeField] GameEvent OnTimeSpeedChange;
    [SerializeField] GameEvent OnSpeedUpTime;
    [SerializeField] GameEvent OnAcceleratedTime;
    [SerializeField] GameEvent OnNormalTime;
    [SerializeField] float NormalTimeValue;
    [SerializeField] float AcceleratedTimeValue;
    [SerializeField] int MinutesToChangeNews;
    [SerializeField] GameEvent OnStopSpeedTimeSound;

    [Header("Time Game Over")]
    [SerializeField] TimeCheckConditional TimeToLose;
    [SerializeField] GameEvent OnElementClick;
    [SerializeField] GameEvent OnGameOverTime;
    [SerializeField] GameEvent OnDisableInput;

    bool isDisableToLoose;

    float TimeVariation;

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
        TimeVariation = NormalTimeValue * 60;

        Day = 00;
        Hour = 04;
        Minute = 25;
    }

    int Seconds;

    int Day;

    int Hour;

    int Minute;

    public int GetActualSeconds()
    {
        return Seconds;
    }

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
    public float GetActuaTimeVariationSpeed()
    {
        return TimeVariation;
    }


    private float secondCounter;

    void Update()
    {
        secondCounter += Time.deltaTime * TimeVariation;

        while (secondCounter >= 1f)
        {
            Seconds++;
            OnSecondsChange?.Invoke();
            secondCounter -= 1f;

            if (Seconds >= 60)
            {
                Minute++;
                Seconds = 0;
                OnMinuteChange?.Invoke();
               // Debug.Log("minute: " + Minute);
                CheckGameOverCondition();

                if (Minute >= 60)
                {
                    Hour++;
                    Minute = 0;
                    OnHourChange?.Invoke();
                }

                if (Minute % MinutesToChangeNews == 0)
                {
                    OnNewsChange?.Invoke();
                }
            }
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
            TimeVariation = AcceleratedTimeValue * 60;
            isTimeAccelerated = true;
            OnTimeSpeedChange?.Invoke(this, AcceleratedTimeValue);
            OnAcceleratedTime?.Invoke(this, true);
            OnSpeedUpTime?.Invoke(this, null);
        }
        else { NormalizeTime(); }
    }

    public void NormalizeTime()
    {
        if (isTimeAccelerated)
        {
            OnStopSpeedTimeSound?.Invoke(this, null);
        }
        TimeVariation = NormalTimeValue * 60;
        isTimeAccelerated = false;
        OnTimeSpeedChange?.Invoke(this, 1f);
        OnAcceleratedTime?.Invoke(this, false);
        OnNormalTime?.Invoke(this, null);
    }

    public void PauseTime()
    {
        TimeVariation = 0;
        OnTimeSpeedChange?.Invoke(this, 0f);
        OnAcceleratedTime?.Invoke(this, false);

    }

    bool once;
    void CheckGameOverCondition()
    {
        if (isDisableToLoose) return;
        if(TimeToLose.GetStateCondition() && !once)
        {
            OnElementClick?.Invoke(this, ViewStates.GameOverView);
            OnDisableInput?.Invoke(this, null);
            OnGameOverTime?.Invoke(this, null);
            once = true;
        }
    }

    public void DisableLoose(Component senddr, object obj)
    {
        isDisableToLoose = true;
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
        return $"{Hour} : {Minute}";
    }

    public  int GetTimeInNum()
    {
        string auxString = Day.ToString() + Hour.ToString("D2") + Minute.ToString("D2");
        int auxInt;
        int.TryParse(auxString, out auxInt);
        return auxInt;
    }
}


