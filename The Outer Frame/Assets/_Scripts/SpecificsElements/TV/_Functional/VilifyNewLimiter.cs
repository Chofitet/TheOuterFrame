using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VilifyNewLimiter : MonoBehaviour
{
    [SerializeField] int MaxVilifyNews;
    [SerializeField] int TimeBetweenVilifyNewsToIncreaseCounter;
    [SerializeField] int TimeToWaitToReactiveVilifyAction;
    [SerializeField] GameEvent LockVilifyNews;
    int VilifyCounter;
    private int minuteProgress;
    private int UnlockMinutePrgress;

    public void SetCounter(Component sender, object obj)
    {
        bool x = (bool)obj;

        if (x) IncreaseCounter();
        else DecreaseCounter();

    }

    void IncreaseCounter()
    {
        //Llamado por el progresor cuando se manda a hacer un vilify
        VilifyCounter += 1;

        VilifyCounter = Mathf.Clamp(VilifyCounter, 0, MaxVilifyNews);

        if (VilifyCounter == MaxVilifyNews)
        {
            SetVilifyLocked();
        }

        SetTimer();
    }

    public void DecreaseCounter()
    {
        VilifyCounter -= 1;
        VilifyCounter = Mathf.Clamp(VilifyCounter, 0, MaxVilifyNews);
        SetVilifyUnlocked(false);

    }

    void SetTimer()
    {
        minuteProgress = 0;
        TimeManager.OnMinuteChange += UpdateTimeProgress;
    }

    void UpdateTimeProgress()
    {
        minuteProgress += 1;

        if (minuteProgress >= TimeBetweenVilifyNewsToIncreaseCounter)
        {
            DecreaseCounter();
            TimeManager.OnMinuteChange -= UpdateTimeProgress;
        }
    }

    void SetVilifyLocked()
    {
        UnlockMinutePrgress = 0;
        TimeManager.OnMinuteChange += UpdateLockedTimeProgress;
        TimeManager.OnMinuteChange -= UpdateTimeProgress;
        TimeData ActualTime = TimeManager.timeManager.GetTime();
        TimeData TimeToUnlockVilify = AddMinutesToTime(ActualTime, TimeToWaitToReactiveVilifyAction);

        LockVilifyNews?.Invoke(this, TimeToUnlockVilify);
        Debug.Log("Vilify locked");
    }

    void UpdateLockedTimeProgress()
    {
        UnlockMinutePrgress += 1;

        if (UnlockMinutePrgress >= TimeToWaitToReactiveVilifyAction)
        {
            SetVilifyUnlocked();
        }
    }
    void SetVilifyUnlocked(bool resetCounter = true)
    {
        LockVilifyNews?.Invoke(this, new TimeData(0,0,0));
        TimeManager.OnMinuteChange -= UpdateLockedTimeProgress;
        if(resetCounter) VilifyCounter = 0;
        Debug.Log("Vilify unlocked");
    }


    private TimeData AddMinutesToTime(TimeData time, int minutesToAdd)
    {
        int totalMinutes = time.Minute + minutesToAdd;
        int extraHours = totalMinutes / 60;
        int finalMinutes = totalMinutes % 60;

        int totalHours = time.Hour + extraHours;
        int extraDays = totalHours / 24;
        int finalHours = totalHours % 24;

        int finalDays = time.Day + extraDays;

        return new TimeData
        {
            Day = finalDays,
            Hour = finalHours,
            Minute = finalMinutes
        };
    }

}
