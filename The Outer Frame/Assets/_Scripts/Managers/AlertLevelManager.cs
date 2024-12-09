using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertLevelManager : MonoBehaviour
{
    [SerializeField] int InitAlertLevel;
    [SerializeField] TMP_Text NumLevel;
    [SerializeField] BlinkMaterialEffect Led;
    [ColorUsage(true, true)] [SerializeField] Color IncreaseColor;
    [ColorUsage(true, true)] [SerializeField] Color DecreaseColor;
    [SerializeField] GameEvent ButtonElement;
    [SerializeField] GameEvent OnGameOverAlert;
    [SerializeField] GameEvent OnUpAlertLevel;
    [SerializeField] GameEvent OnDownAlertLevel;
    int level;
    bool isStoped;

    private void OnEnable()
    {
        level = InitAlertLevel;
    }


    public void UpdateNum(Component sender, object obj)
    {
        if (isStoped) return;
        int incruseNum = (int)obj;
        level = level + incruseNum;

        if (level < 0) level = 1;
        NumLevel.text = level + "%";

        if (level >= 100)
        {
            Invoke("end", 0.2f);
            return;
        }

        if (incruseNum > 0)
        {
            Led.SetSpecificColor(IncreaseColor);
            OnUpAlertLevel?.Invoke(this, null);
        }
        else if (incruseNum < 0)
        {
            Led.SetSpecificColor(DecreaseColor);
            OnDownAlertLevel?.Invoke(this, null);
        }


    }
    private void end()
    {
        OnGameOverAlert?.Invoke(this, null);
        ButtonElement?.Invoke(this, ViewStates.GameOverView);
    }

    public void StopAlertLevelManager(Component sender, object obj)
    {
        isStoped = true;
    }
}
