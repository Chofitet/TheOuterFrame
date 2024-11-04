using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text Resulttxt;
    [SerializeField] TMP_Text ActionCalltxt;
    [SerializeField] TMP_Text Statustxt;
    [SerializeField] GameEvent OnMovePaperToTakenPos;
    [SerializeField] float DelayToPC;
    [SerializeField] TMP_Text btnText;
    [SerializeField] PhotoReportSetter photo1;
    [SerializeField] PhotoReportSetter photo2;
    [SerializeField] PhotoReportSetter photo3;
    [SerializeField] Sprite ThumbUp;
    [SerializeField] List<Sprite> WrongResultImg = new List<Sprite>();
    
    bool isNotCompleted;
    WordData word;
    ReportType report;

    public void initReport(WordData _word, ReportType _report, bool isAborted, bool isAlreadyDone, bool isTheSameAction, StateEnum isOtherActionInGroupDoing, TimeData timeComplete)
    {
        word = _word;
        report = _report;
        isNotCompleted = false;
        string status = "<color=#006A0D>COMPLETED</color>";
        btnText.text = "UPLOAD TO DB";
        StateEnum state = report.GetAction();
        string Name = word.GetForm_DatabaseNameVersion();
        if (state.GetSpecialActionWord()) Name = "";
        string actionVerb = state.GetInfinitiveVerb();

        if (!report)
        {
            Resulttxt.text = "No report not assigned in " + Name;
            status = "a";
            isNotCompleted = true;
            btnText.text = "DISPOSE";
        }
        else if (isAlreadyDone)
        {
            Resulttxt.text = report.GetTextForRepetition();
            photo1.Set("REMEMBER TO READ", WrongResultImg[new System.Random().Next(2) == 0 ? 5 : 8]);
            if (report.GetTextForRepetition() == "") Debug.LogWarning("No text for repetition in report: " + report.name);
            status = "<color=#AE0000>REJECTED</color>";
            isNotCompleted = true;
            btnText.text = "DISPOSE";
        }
        else if (isTheSameAction)
        {
            Resulttxt.text = "We are already doing that exact same thing.";
            status = "<color=#AE0000>REJECTED</color>";
            photo1.Set("JUST HAVE TO WAIT", WrongResultImg[new System.Random().Next(2) == 0 ? 6 : 8]);
            isNotCompleted = true;
            btnText.text = "DISPOSE";
        }
        else if(isOtherActionInGroupDoing != null)
        {
            Resulttxt.text = "We are currently " + isOtherActionInGroupDoing.GetActioningVerb() + " " + Name + ".\n\rWe'll have to be done with THAT first.";
            status = "<color=#AE0000>REJECTED</color>";
            photo1.Set("", WrongResultImg[6]);
            isNotCompleted = true;
            btnText.text = "DISPOSE";
        }
        else if(isAborted)
        {
            Resulttxt.text = "The action \"" + actionVerb + " " + Name + "\" was aborted succesfully";
            status = "<color=#AE0000>ABORTED</color>";
            photo1.Set("", WrongResultImg[7]);
            isNotCompleted = true;
            btnText.text = "DISPOSE";
        }
        else if (report.GetIsAutomatic())
        {
            status = "<color=#AE0000>REJECTED</color>";
            
            btnText.text = "DISPOSE";
        }

        ActionCalltxt.text = actionVerb + " " + DeleteSpetialCharacter(Name);
        CheckTextOverflow();
        Statustxt.text = status + " at OCT 30th " + $"{timeComplete.Hour:00}:{timeComplete.Minute:00}";

        GetComponent<IndividualReportController>().SetType(false, word, report);

        if (isNotCompleted) return;
        Resulttxt.text = report.GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(Resulttxt,FindableBtnType.FindableBTN);
        GetComponent<IndividualReportController>().SetType(true, word, report);

        SetPhotos(report);

    }

    void SetPhotos(ReportType report)
    {
        List<PhotoInfo> photoInfo = report.GetReportImage();

        if(photoInfo.Count >= 1) photo1.Set(photoInfo[0]?.text, photoInfo[0]?.photo);
        if (photoInfo.Count >= 2) photo2.Set(photoInfo[1]?.text, photoInfo[1]?.photo);
        if (photoInfo.Count >= 3) photo3.Set(photoInfo[2]?.text, photoInfo[2]?.photo);
    }

    string DeleteSpetialCharacter(string txt)
    {
        return Regex.Replace(txt, @"[\?\.,\n\r]", "");
    }

    public void OnTakeReport(Component sender, object obj)
    {
        GetComponent<BoxCollider>().enabled = true;
        OnMovePaperToTakenPos?.Invoke(this, gameObject);
        if(!isNotCompleted) WordsManager.WM.RequestChangeStateSeen(word, report.GetState());
        Destroy(this);
    }

    void CheckTextOverflow()
    {
        float containerWidth = ActionCalltxt.transform.parent.GetComponent<RectTransform>().rect.width;
        Vector2 preferredSize = ActionCalltxt.GetPreferredValues();

        if (preferredSize.x > containerWidth) TruncateTextWithEllipsis(containerWidth);
        else ActionCalltxt.text = ActionCalltxt.text.Replace(" . . .", "");
    }

    void TruncateTextWithEllipsis(float containerWidth)
    {
        string originalText = ActionCalltxt.text;
        string truncatedText = originalText;

        while (ActionCalltxt.GetPreferredValues(truncatedText + " . . .").x > containerWidth && truncatedText.Length > 0)
        {
            truncatedText = truncatedText.Substring(0, truncatedText.Length - 1);
        }

        ActionCalltxt.text = truncatedText + " . . .";
    }

    public void _Reset()
    {
        photo1.gameObject.SetActive(false);
        photo2.gameObject.SetActive(false);
        photo3.gameObject.SetActive(false);
    }

}
