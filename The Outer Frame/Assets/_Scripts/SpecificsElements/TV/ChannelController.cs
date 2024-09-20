using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{

    bool isFull;
    [SerializeField] bool isStaticChannel;
    [SerializeField] OverlayAnimation OverlayAnims;
    [SerializeField] TMP_Text EmergencyTextField;
    [SerializeField] GameObject EmergencyScreen;
    [SerializeField] TMP_Text HeadlineText;
    [SerializeField] TMP_Text NewTextContent;
    [SerializeField] Image NewImg;
    [SerializeField] string TriggerAnim;
    [SerializeField] GameEvent OnIncreaseAlertLevel;
    [SerializeField] GameEvent OnChangeReporterAnim;

    public bool GetIsFull() { return isFull; }

    public void SetIsFull(bool x) => isFull = x;

    //voy a hacer que el canal sea el que inicie contadores de cuanto tiempo dejará una noticia.
    //deberá gestionar una cola de noticias que le entran y partir los bloques que ya creo.
    //

    public void SetNew(INewType _new)
    {
        if (_new == null) return;
        EmergencyScreen.SetActive(false);
       
        OverlayAnims.NewsOut();
        OverlayAnims.PicsOut();
        OverlayAnims.QuipOut();
        OnChangeReporterAnim?.Invoke(this, null);

        StartCoroutine(BackUI(OverlayAnims.GetAnimTime(), _new));
       
    }

    IEnumerator BackUI(float time, INewType _new)
    {
        yield return new WaitForSeconds(time);

        Debug.Log("time to change new: " + time);

        OverlayAnims.NewsIn();
        OverlayAnims.PicsIn();
        OverlayAnims.QuipIn();

        HeadlineText.text = _new.GetHeadline();
        NewTextContent.text = _new.GetNewText();
        if(_new.GetNewText() == "") NewTextContent.text = _new.GetHeadline();
        NewImg.sprite = _new.GetNewImag();
        if (_new.GetIfIsAEmergency()) ChangeToEmergencyLayout(_new);
        else FindableWordsManager.FWM.InstanciateFindableWord(HeadlineText);
        OnIncreaseAlertLevel?.Invoke(this, _new.GetIncreaseAlertLevel());

    }

    void ChangeToEmergencyLayout(INewType _new)
    {
        EmergencyScreen.SetActive(true);
        EmergencyTextField.text = _new.GetHeadline();
        FindableWordsManager.FWM.InstanciateFindableWord(EmergencyTextField);
    }

    public void resetChannel()
    {
       /* EmergencyScreen.SetActive(false);
        HeadlineText.text = "We runout of news, stay tuned for more";
        NewImg.color = new Color(1, 1, 1, 0);*/
    }

    public bool GetisStaticChannel() { return isStaticChannel; }

    public string GetTriggerAnim() { return TriggerAnim; }

}
