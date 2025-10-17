using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayStyleController : MonoBehaviour
{
    public List<TVOverlayStyle> TVOverlayStyles = new List<TVOverlayStyle>();

    public void ChangeStyleChannel(string styleName)
    {
        foreach (TVOverlayStyle style in TVOverlayStyles)
        {
            bool isTarget = style.name == styleName;
            style.SetActive(isTarget);
        }
    }
}


[Serializable]
public class TVOverlayStyle
{
    public string name;
    public GameObject Quip;
    public GameObject Title;
    public GameObject Text;
    public GameObject DownPanel;
    public GameObject Logo;
    public GameObject ImgQuip;
    public GameObject Reporter;
    public SpriteRenderer ReporterHead;
    public SpriteRenderer ReporterHands;

    public void SetActive(bool x)
    {
        if (Quip) Quip.SetActive(x);
        if (Title) Title.SetActive(x);
        if (Text) Text.SetActive(x);
        if (DownPanel) DownPanel.SetActive(x);
        if (Logo) Logo.SetActive(x);
        if (ImgQuip) ImgQuip.SetActive(x);
        if(Reporter) Reporter.SetActive(x);
        if(ReporterHead) ReporterHead.enabled = x;
        if(ReporterHands) ReporterHands.enabled = x;
    }

}