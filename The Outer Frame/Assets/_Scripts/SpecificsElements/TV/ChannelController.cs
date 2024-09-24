using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{

    bool isFull;
    [SerializeField] bool isStaticChannel;
    [SerializeField] int DefaultMinutesToPassNews;
    [SerializeField] OverlayAnimation OverlayAnims;
    [SerializeField] TMP_Text EmergencyTextField;
    [SerializeField] GameObject EmergencyScreen;
    [SerializeField] TMP_Text HeadlineText;
    [SerializeField] TMP_Text HeadlineText2;
    [SerializeField] TMP_Text NewTextContent;
    [SerializeField] Image NewImg;
    [SerializeField] string TriggerAnim;
    [SerializeField] GameEvent OnIncreaseAlertLevel;
    [SerializeField] GameEvent OnChangeReporterAnim;
    TimeCheckConditional MinTimeToShowNew;
    TimeCheckConditional TimeToRestartRandoms;


    private void Start()
    {
        if (isStaticChannel) isFull = true;
        
    }

    public bool GetIsFull() { return isFull; }

    public void SetIsFull(bool x) => isFull = x;

    public bool GetIsMinTimePass() 
    {
        if (!MinTimeToShowNew) return true;
        return MinTimeToShowNew.GetStateCondition();
    }

    public bool GetTimeToRestartRandoms() 
    {
        if (!TimeToRestartRandoms) return true; 
        return TimeToRestartRandoms.GetStateCondition(); 
    }

    //voy a hacer que el canal sea el que inicie contadores de cuanto tiempo dejará una noticia.
    //deberá gestionar una cola de noticias que le entran y partir los bloques que ya creo.
    //

    public void SetNew(INewType _new)
    {
        if (_new == null) return;
        EmergencyScreen.SetActive(false);

        MinTimeToShowNew = DefineTime(MinTimeToShowNew, _new.GetMinTransmitionTime());
        TimeToRestartRandoms = DefineTime(TimeToRestartRandoms, DefaultMinutesToPassNews);


        OverlayAnims.NewsOut();
        OverlayAnims.PicsOut();
        OverlayAnims.QuipOut();
        OnChangeReporterAnim?.Invoke(this, null);

        StartCoroutine(BackUI(OverlayAnims.GetAnimTime(), _new));

        _new.SetWasStreamed();

        Debug.Log("The new " + _new.GetHeadline() + " was setted in channel " + name + " At " + TimeManager.timeManager.GetTime().ToString() +
            "\n" + "MINIMUM hour to show: " + MinTimeToShowNew.GetTimeSetted().ToString() +
            "\n" + "MAXUMUM hour to show: " + TimeToRestartRandoms.GetTimeSetted().ToString());
    }

    IEnumerator BackUI(float time, INewType _new)
    {
        yield return new WaitForSeconds(time);

        

        OverlayAnims.NewsIn();
        OverlayAnims.PicsIn();
        OverlayAnims.QuipIn();

        if (_new.GetHeadline2() != "")
        {
            HeadlineText2.gameObject.SetActive(true);
            HeadlineText.gameObject.SetActive(false);
            HeadlineText2.text = _new.GetHeadline2();

        }
        else
        {
            HeadlineText2.gameObject.SetActive(false);
            HeadlineText.gameObject.SetActive(true);
            HeadlineText.text = _new.GetHeadline();
        }
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

    TimeCheckConditional DefineTime(TimeCheckConditional timeConditional, int minutes)
    {
        TimeData ActualTime = TimeManager.timeManager.GetTime();

        TimeData FixedEndTime = AddMinutesToTime(ActualTime, minutes);
        timeConditional = ScriptableObject.CreateInstance<TimeCheckConditional>();
        timeConditional.Initialize(true, FixedEndTime.Day, FixedEndTime.Hour, FixedEndTime.Minute);

        return timeConditional;
    }

    private TimeData AddMinutesToTime(TimeData time, int minutesToAdd)
    {
        int totalMinutes = time.Minute + minutesToAdd;
        int extraHours = totalMinutes / 60;
        int finalMinutes = totalMinutes % 60;

        int totalHours = time.Hour + extraHours;
        int extraDays = totalHours / 24;
        int finalHours = totalHours % 24;

        int finalDays = time.Day + extraDays;

        return new TimeData
        {
            Day = finalDays,
            Hour = finalHours,
            Minute = finalMinutes
        };
    }
}
