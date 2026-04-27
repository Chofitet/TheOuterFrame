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

    [SerializeField] GameEvent OnMinutePass;
    [SerializeField] GameEvent OnSecondPass;

    [Header("Time Game Over")]
    [SerializeField] TimeCheckConditional TimeToLose;
    [HideInInspector][SerializeField] GameEvent OnElementClick;
    [HideInInspector][SerializeField] GameEvent OnGameOverTime;
    [HideInInspector][SerializeField] GameEvent OnDisableInput;


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
            OnSecondPass?.Invoke(this, null);
            secondCounter -= 1f;

            if (Seconds >= 60)
            {
                Minute++;
                Seconds = 0;
                OnMinuteChange?.Invoke();
                OnMinutePass?.Invoke(this, null);
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

    public void SetAnotherSpeed(float _speed)
    {
        TimeVariation = _speed;
        OnTimeSpeedChange?.Invoke(this, _speed);
        OnAcceleratedTime?.Invoke(this, false);
    }

    bool onceTimeToLose;
    bool onceTimeToCutDemo;
    void CheckGameOverCondition()
    {
        if (isDisableToLoose) return;
        if(TimeToLose.GetStateCondition() && !onceTimeToLose)
        {
            OnElementClick?.Invoke(this, ViewStates.GameOverView);
            OnDisableInput?.Invoke(this, null);
            OnGameOverTime?.Invoke(this, null);
            onceTimeToLose = true;
        }
        
    }

    public void DisableLoose(Component senddr, object obj)
    {
        isDisableToLoose = true;
    }

    public TimeData SubtractMinutesFromTime(TimeData time, int minutesToSubtract)
    {
        int totalMinutes = time.Minute - minutesToSubtract;

        // Si los minutos son negativos, calculamos las horas que hay que restar
        int extraHours = 0;
        while (totalMinutes < 0)
        {
            totalMinutes += 60;
            extraHours++;
        }

        int finalMinutes = totalMinutes;

        int totalHours = time.Hour - extraHours;
        int extraDays = 0;
        while (totalHours < 0)
        {
            totalHours += 24;
            extraDays++;
        }

        int finalHours = totalHours;
        int finalDays = time.Day - extraDays;

        // No hacemos wrap-around de días negativos (depende de tu lógica de tiempo)
        return new TimeData
        {
            Day = finalDays,
            Hour = finalHours,
            Minute = finalMinutes
        };
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
        string _min = $"{Minute:00}";
        string _hour = $"{Hour:00}";

        return $"{_hour}:{_min}";
    }

    public  int GetTimeInNum()
    {
        string auxString = Day.ToString() + Hour.ToString("D2") + Minute.ToString("D2");
        int auxInt;
        int.TryParse(auxString, out auxInt);
        return auxInt;
    }

    public bool isANullTimeData()
    {
        if (Day == 0 && Hour == 0 && Minute == 0) return true;
        else return false;
    }

   
}


