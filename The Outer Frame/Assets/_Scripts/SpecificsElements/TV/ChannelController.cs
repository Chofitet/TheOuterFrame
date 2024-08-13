using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{
    bool isFull;
    [SerializeField] TMP_Text EmergencyTextField;
    [SerializeField] GameObject EmergencyScreen;
    [SerializeField] TMP_Text HeadlineText;
    [SerializeField] Image NewImg;
    [SerializeField] string TriggerAnim;
    public bool GetIsFull() { return isFull; }

    public void SetIsFull(bool x) => isFull = x;

    public void SetNew(INewType _new)
    {
        if (_new == null) return;
        HeadlineText.gameObject.SetActive(true);
        HeadlineText.text = _new.GetHeadline();
        NewImg.sprite = _new.GetNewImag();
        if (!_new.GetNewImag()) NewImg.color = new Color(1, 1, 1, 0);
        else NewImg.color = new Color(1, 1, 1, 1);
        EmergencyTextField.text = _new.GetHeadline();
        if (_new.GetIfIsAEmergency()) ChangeToEmergencyLayout();
        else FindableWordsManager.FWM.InstanciateFindableWord(HeadlineText);
    }

    void ChangeToEmergencyLayout()
    {
        EmergencyScreen.SetActive(true);
        FindableWordsManager.FWM.InstanciateFindableWord(EmergencyTextField);
    }

    public void resetChannel()
    {
        EmergencyScreen.SetActive(false);
        HeadlineText.text = "We runout of news, stay tuned for more";
        NewImg.color = new Color(1, 1, 1, 0);
    }


    public string GetTriggerAnim() { return TriggerAnim; }

}
