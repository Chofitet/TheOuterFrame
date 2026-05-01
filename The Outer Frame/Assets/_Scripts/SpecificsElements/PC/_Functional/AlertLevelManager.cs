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
    [SerializeField] bool NoLimitToGameOver;
    [SerializeField] GameEvent Oninstanciatepopup;
    [SerializeField] TVNewType WishlistNew;
    [SerializeField] GameEvent On1000Wishlists;
    [SerializeField] GameObject Mas;
    float timeFactor = 1;
    int level;
    bool isStoped;
    int maxLevel = 100;

    private void OnEnable()
    {
        //level = InitAlertLevel;
    }


    [ContextMenu("1000 wishlists")]
    public void MilWishlists()
    {
        Invoke("startIncruise", 1.3f);
        Invoke("Mil", 2.9f);
        Invoke("WishlistReach", 3.6f);
        On1000Wishlists?.Invoke(this, null);
    }

    void startIncruise()
    {
        UpdateNum(null, new AlertData(1000, "THANK YOU ALL!!!"));
    }

    void Mil()
    {
        Mas.SetActive(true);
    }

    void WishlistReach()
    {
        Oninstanciatepopup?.Invoke(this, WishlistNew as IPopUp);
    }

    public void UpdateNum(Component sender, object obj)
    {
        if (isStoped) return;
        AlertData data = (AlertData)obj;
        int auxIncruise = level + data.IncruseNum;
        if(data.AclarationText != "") AclarationText.text = data.AclarationText;
        /*if (level < 15) level = 15;
        if(auxIncruise < 15) auxIncruise = 15;*/
       // Debug.Log($"Level to incruise {level}");
        DOTween.To(() => level, x => level = x, auxIncruise, 0.8f / timeFactor).SetEase(Ease.InSine).OnComplete(() => { 
            /*if (auxIncruise >= maxLevel)
            {
                Invoke("end", 0.2f);
                isStoped = true;
            }*/
            }); 

       /* if (auxIncruise >= 100)
        {
            return;
        }*/

        if (data.IncruseNum < 0)
        {
            Led.SetSpecificColor(IncreaseColor);
            OnUpAlertLevel?.Invoke(this, null);
        }
        else if (data.IncruseNum > 0)
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
        NumLevel.text = level.ToString();

        if (Input.GetKeyDown(KeyCode.W))
           {
            MilWishlists();
        }


        if (Input.GetKeyDown(KeyCode.G) && NoLimitToGameOver)
        {
            maxLevel = 1000;
            NumLevel.text = "1000%";
        }
    }

    public void AccelerateAnims(Component sender, object obj)
    {
        timeFactor = (float)obj;
    }

    [ContextMenu("Try Alert Level")]
    public void AlertLevelTry()
    {
        UpdateNum(null, 1);
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