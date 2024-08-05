using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{
    bool isFull;
    [SerializeField] TMP_Text HeadlineText;
    [SerializeField] Image NewImg;
    [SerializeField] string TriggerAnim;
    public bool GetIsFull() { return isFull; }

    public void SetIsFull(bool x) => isFull = x;

    public void SetNew(INewType _new)
    {
        HeadlineText.gameObject.SetActive(true);
        HeadlineText.text = _new.GetHeadline();
        NewImg.sprite = _new.GetNewImag();
        if (!_new.GetNewImag()) NewImg.color = new Color(1, 1, 1, 0);
        else NewImg.color = new Color(1, 1, 1, 1);
        FindableWordsManager.FWM.InstanciateFindableWord(HeadlineText);
    }

    public string GetTriggerAnim() { return TriggerAnim; }

}
