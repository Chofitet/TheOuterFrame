using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimePanelController : MonoBehaviour
{
    [SerializeField] TMP_Text textField;

    private void OnEnable()
    {
        TimeManager.OnMinuteChange += UpdateTime;
        TimeManager.OnHourChange += UpdateTime;
    }


    private void OnDisable()
    {
        TimeManager.OnMinuteChange -= UpdateTime;
        TimeManager.OnHourChange -= UpdateTime;
    }

    private void Start()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        textField.text = $"{TimeManager.timeManager.GetActualHour():00}:{TimeManager.timeManager.GetActualMinute():00}";
    }

}
