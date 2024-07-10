using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChannelController : MonoBehaviour
{
    bool isFull;
    [SerializeField] TMP_Text HeadlineText;
    public bool GetIsFull() { return isFull; }

    public void SetIsFull(bool x) => isFull = x;

    public void SetNew(INewType _new)
    {
        HeadlineText.text = _new.GetHeadline();
        FindableWordsManager.FWM.InstanciateFindableWord(HeadlineText);
    }

}
