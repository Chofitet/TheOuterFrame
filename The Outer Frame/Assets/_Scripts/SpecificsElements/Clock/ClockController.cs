using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockController : MonoBehaviour
{
    [SerializeField] TMP_Text datetxt;
    
    [SerializeField] TMP_Text MinuteMovibleFlipFront;
    [SerializeField] TMP_Text MinuteInamovibleFlipFront;
    [SerializeField] TMP_Text MinuteMovibleFlipBack;
    [SerializeField] TMP_Text MinuteInamovibleFlipBack;
    [SerializeField] AnimationClip MinuteAnim;

    [SerializeField] TMP_Text HourMovibleFlipFront;
    [SerializeField] TMP_Text HourInamovibleFlipFront;
    [SerializeField] TMP_Text HourMovibleFlipBack;
    [SerializeField] TMP_Text HourInamovibleFlipBack;
    [SerializeField] AnimationClip HourAnim;

    TimeManager TM;
    Animator anim;
    bool isSpeedUp;
    private void Start()
    {
        TM = TimeManager.timeManager;
        anim = GetComponent<Animator>();

        Minute = $"{TM.GetActualMinute():00}";

        MinuteMovibleFlipFront.text = Minute;
        MinuteInamovibleFlipFront.text = Minute;
        MinuteInamovibleFlipBack.text = Minute;
        MinuteMovibleFlipBack.text = Minute;

        Hour = $"{TM.GetActualHour():00}";

        HourMovibleFlipFront.text = Hour;
        HourInamovibleFlipFront.text = Hour;
        HourInamovibleFlipBack.text = Hour;
        HourMovibleFlipBack.text = Hour;
    }

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateMinuteClock;
        TimeManager.OnHourChange += UpdateHourClock;
    }
    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= UpdateMinuteClock;
        TimeManager.OnHourChange -= UpdateHourClock;
    }

    string Minute;

    private void UpdateMinuteClock()
    {
        Minute = $"{TM.GetActualMinute():00}";

        
        string PreviousMinute = $"{TM.GetActualMinute() - 1:00}";

        MinuteMovibleFlipFront.text = Minute;
        MinuteInamovibleFlipFront.text = Minute;

        MinuteInamovibleFlipBack.text = PreviousMinute;
        MinuteMovibleFlipBack.text = PreviousMinute;

        Invoke("ChangeBackMinute", MinuteAnim.length);

        anim.SetTrigger("minute");
    }

    void ChangeBackMinute()
    {
        MinuteInamovibleFlipBack.text = Minute;
        MinuteMovibleFlipBack.text = Minute;
    }

    string Hour;
    private void UpdateHourClock()
    {
        Hour = $"{TM.GetActualHour():00}";
        
        string PreviousHour = $"{TM.GetActualHour() - 1:00}";

        HourMovibleFlipFront.text = Hour;
        HourInamovibleFlipFront.text = Hour;

        HourInamovibleFlipBack.text = PreviousHour;
        HourMovibleFlipBack.text = PreviousHour;

        Invoke("ChangeBackHour", HourAnim.length);

        anim.SetTrigger("hour");
    }

    void ChangeBackHour()
    {
        HourInamovibleFlipBack.text = Hour;
        HourMovibleFlipBack.text = Hour;
    }

    ViewStates currentView;
    public void CheckView(Component sender, object obj)
    {
        currentView = (ViewStates)obj;
    }

    private void OnMouseDown()
    {
        if (currentView != ViewStates.GeneralView) return;

        TimeManager.timeManager.SpeedUpTime();
        
        if(!isSpeedUp)
        {
            anim.SetTrigger("speedUp");
            isSpeedUp = true;
        }
        else
        {
            anim.SetTrigger("speedDown");
            isSpeedUp = false;
        }

    }

    public void SetClockSpeed(Component sender, object obj)
    {
        float TimeVariation = (float)obj;

        anim.SetFloat("clockSpeed", TimeVariation);
    }

}
