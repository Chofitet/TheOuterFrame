using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertLevelManager : MonoBehaviour
{
    [SerializeField] TMP_Text NumLevel;
    [SerializeField] BlinkEffect Led;
    int level;

    public void UpdateNum(Component sender, object obj)
    {
        int incruseNum = (int)obj;
        level = level + incruseNum;

        NumLevel.text = level + "%";

        if (incruseNum > 0)
        {
            Led.ActiveBlink(null, null);
            Invoke("TurnOffLigth", 1.5f);
        }
        else if (incruseNum < 0)
        {
            Led.ActiveBlink(null, null);
            Invoke("TurnOffLigth", 1.5f);
        }
    }

    void TurnOffLigth()
    {
        Led.TurnOffLigth(null, null);
    }
}
