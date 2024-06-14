using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockController : MonoBehaviour
{
    [SerializeField] TMP_Text datetxt;
    [SerializeField] TMP_Text timetxt;
    TimeManager TM;
    private void Start()
    {
        TM = TimeManager.timeManager;
    }

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateClock;
    }
    private void OnDisable()
    {
        TimeManager.OnMinuteChange += UpdateClock;
    }

    private void UpdateClock()
    {
        timetxt.text = $"{TM.GetActualHour():00}:{TM.GetActualMinute():00}";
    }

    private void OnMouseDown()
    {
        TimeManager.timeManager.SpeedUpTime();
    }
}
