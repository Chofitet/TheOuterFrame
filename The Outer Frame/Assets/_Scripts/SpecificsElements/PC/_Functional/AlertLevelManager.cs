using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertLevelManager : MonoBehaviour
{
    [SerializeField] int InitAlertLevel;
    [SerializeField] TMP_Text NumLevel;
    [SerializeField] TMP_Text AclarationText;
    [SerializeField] BlinkMaterialEffect Led;
    [ColorUsage(true, true)] [SerializeField] Color IncreaseColor;
    [ColorUsage(true, true)] [SerializeField] Color DecreaseColor;
    [SerializeField] GameEvent ButtonElement;
    [SerializeField] GameEvent OnGameOverAlert;
    [SerializeField] GameEvent OnUpAlertLevel;
    [SerializeField] GameEvent OnDownAlertLevel;
    float timeFactor = 1;
    int level;
    bool isStoped;
    int maxLevel = 100;

    private void OnEnable()
    {
        level = InitAlertLevel;
    }


    public void UpdateNum(Component sender, object obj)
    {
        if (isStoped) return;
        AlertData data = (AlertData)obj;
        int auxIncruise = level + data.IncruseNum;
        if(data.AclarationText != "") AclarationText.text = data.AclarationText;
        if (level < 0) level = 1;
        DOTween.To(() => level, x => level = x, auxIncruise, 0.8f / timeFactor).SetEase(Ease.InSine).OnComplete(() => { 
            if (auxIncruise >= maxLevel)
            {
                Invoke("end", 0.2f);
            }
            }); 

        if (auxIncruise >= 100)
        {
            return;
        }

        if (data.IncruseNum > 0)
        {
            Led.SetSpecificColor(IncreaseColor);
            OnUpAlertLevel?.Invoke(this, null);
        }
        else if (data.IncruseNum < 0)
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

    private void Update()
    {
        NumLevel.text = level + "%";

      /* if (Input.GetKeyDown(KeyCode.G))
        {
            maxLevel = 1000;
            NumLevel.text = "1000%";
        }*/
    }



    public void AccelerateAnims(Component sender, object obj)
    {
        timeFactor = (float)obj;

        
    }


}


public class AlertData
{
    public int IncruseNum;
    public string AclarationText;

    public AlertData(int incruseNum, string aclarationText)
    {
        IncruseNum = incruseNum;
        AclarationText = aclarationText;
    }
}