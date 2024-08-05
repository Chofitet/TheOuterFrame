using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChannelController : MonoBehaviour
{
    bool isFull;
    [SerializeField] TMP_Text HeadlineText;
    [SerializeField] string TriggerAnim;
    public bool GetIsFull() { return isFull; }

    public void SetIsFull(bool x) => isFull = x;

    public void SetNew(INewType _new)
    {
        HeadlineText.gameObject.SetActive(true);
        HeadlineText.text = _new.GetHeadline();
        FindableWordsManager.FWM.InstanciateFindableWord(HeadlineText);
    }

    public string GetTriggerAnim() { return TriggerAnim; }

}
