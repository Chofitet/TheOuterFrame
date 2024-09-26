using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertLevelManager : MonoBehaviour
{
    [SerializeField] TMP_Text NumLevel;
    [SerializeField] BlinkMaterialEffect Led;
    [SerializeField] GameEvent ButtonElement;
    [SerializeField] GameEvent OnUpAlertLevel;
    [SerializeField] GameEvent OnDownAlertLevel;
    int level;

    

    public void UpdateNum(Component sender, object obj)
    {
        int incruseNum = (int)obj;
        level = level + incruseNum;

        NumLevel.text = level + "%";

        if (incruseNum > 0)
        {
            OnUpAlertLevel?.Invoke(this, null);
        }
        else if (incruseNum < 0)
        {
            OnDownAlertLevel?.Invoke(this, null);
        }

        if(level >= 100)
        {
            end();
        }
    }
    private void end()
    {
        ButtonElement?.Invoke(this, ViewStates.GameOverView);
    }

    [ContextMenu("TestGameOver")]
    private void TestgameOver()
    {
        Invoke("end", 5);
    }
}
