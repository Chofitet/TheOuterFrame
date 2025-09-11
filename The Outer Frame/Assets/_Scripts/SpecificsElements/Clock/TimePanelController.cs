using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimePanelController : MonoBehaviour
{
    [SerializeField] TMP_Text textField;
    bool once;

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateTime;
        TimeManager.OnHourChange += UpdateTime;
        if (once) UpdateTime();
    }


    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= UpdateTime;
        TimeManager.OnHourChange -= UpdateTime;
    }

    private void Start()
    {
        UpdateTime();
        once = true;
    }

    private void UpdateTime()
    {
        textField.text = $"{TimeManager.timeManager.GetActualHour():00}:{TimeManager.timeManager.GetActualMinute():00}";
    }

}
