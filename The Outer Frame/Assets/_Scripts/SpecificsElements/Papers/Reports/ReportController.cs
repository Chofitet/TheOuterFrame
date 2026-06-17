using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class ReportController : MonoBehaviour
{
    [SerializeField] TMP_Text Resulttxt;
    [SerializeField] TMP_Text ActionCalltxt;
    [SerializeField] TMP_Text Statustxt;
    [SerializeField] TMP_Text Hourtxt;
    [SerializeField] GameEvent OnMovePaperToTakenPos;
    [SerializeField] float DelayToPC;
    [SerializeField] TMP_Text btnText;
    [SerializeField] PhotoReportSetter photo1;
    [SerializeField] PhotoReportSetter photo2;
    [SerializeField] PhotoReportSetter photo3;
    [SerializeField] PhotoReportSetter photo4;
    [SerializeField] PhotoReportSetter photo5;
    [SerializeField] PhotoReportSetter photoQR;
    [SerializeField] Sprite ThumbUp;
    [SerializeField] Transform OutPos;
    [SerializeField] List<Sprite> WrongResultImg = new List<Sprite>();
    [SerializeField] GameObject UploadBTN;
    [SerializeField] GameObject DisposeBTN;

    [Header("MaterialSettings")]
    [SerializeField] GameObject pageModel;
    [SerializeField] Material materialUploadDB;
    [SerializeField] Material materialDispose;

    bool isNotCompleted;
    WordData word;
    ReportType report;
    bool IsAlreadyImposible;

    public void initReport(WordData _word, ReportType _report, bool isAborted, bool isAlreadyDone, bool isTheSameAction, StateEnum isOtherActionInGroupDoing, bool isAlreadyImposible, TimeData timeComplete,TimeData TimeToUnlockVilify, int AgentsUsed)
    {
        word = _word;
        report = _report;
        isNotCompleted = false;
        string status = "<color=#006A0D>COMPLETED</color>";
        if (report.GetCustomStatus() != "") status = report.GetCustomStatus();
        UploadBTN.SetActive(true);
        StateEnum state = report.GetAction();
        string Name = word.GetFormNameVersion();
        if (state.GetSpecialActionWord()) Name = "";
        string actionVerb = state.GetInfinitiveVerb();
        SetMaterial(materialDispose);
        IsAlreadyImposible = isAlreadyImposible;

        if (!report)
        {
            Resulttxt.text = "No report not assigned in " + Name;
            status = "a";
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if (isAlreadyDone)
        {
            Resulttxt.text = report.GetTextForRepetition();
            FindableWordsManager.FWM.InstanciateFindableWord(Resulttxt, FindableBtnType.FindableBTN, report.FindableWords,false,false,true);
            photo1.Set("REMEMBER TO READ", WrongResultImg[new System.Random().Next(2) == 0 ? 5 : 8]);
            if (report.GetTextForRepetition() == "") Debug.LogWarning("No text for repetition in report: " + report.name);
            status = "<color=#AE0000>REDUNDANT</color>";
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if (isTheSameAction)
        {
            Resulttxt.text = "We are already doing that exact same thing.";
            status = "<color=#AE0000>REDUNDANT</color>";
            photo1.Set("REMEMBER?", WrongResultImg[8]);
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if (isOtherActionInGroupDoing != null)
        {
            Resulttxt.text = "We are currently " + isOtherActionInGroupDoing.GetActioningVerb() + " " + word.GetFormNameVersion() + ".\n\r\n\rWe'll have to be done with THAT first.";
            status = "<color=#AE0000>NOT RIGHT NOW</color>";
            photo1.Set("JUST A MOMENT", WrongResultImg[6]);
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if (isAborted)
        {
            Resulttxt.text = "The action " + actionVerb + " " + Name + " was aborted successfully.";
            status = "<color=#AE0000>ABORTED</color>";
            photo1.Set("AS YOU SAID", WrongResultImg[7]);
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if (report.GetIsAutomatic())
        {
            status = "<color=#AE0000>IMPOSSIBLE</color>";

            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if (isAlreadyImposible)
        {
            Resulttxt.text = "We could have done that earlier, but the target’s state has changed.";
            status = "<color=#AE0000>NO LONGER POSSIBLE</color>";
            photo1.Set("MAYBE IN ANOTHER LIFE", WrongResultImg[new System.Random().Next(3, 5)]);
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);

        }
        else if(!TimeToUnlockVilify.isANullTimeData())
        {
            Resulttxt.text = $"People will get suspicious if we put so many of our ‘broadcasts’ up one after the other.\n\nLet’s try again after {TimeToUnlockVilify.Hour:00}:{TimeToUnlockVilify.Minute:00}.";
            isNotCompleted = true;
            photo1.Set("LET'S NOT ABUSE IT", WrongResultImg[6]);
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        else if(AgentsUsed < state.GetAgentsNeeded() && AgentsUsed != -1)
        {
            Resulttxt.text = $"There are not enough agents available.\r\nWe need {state.GetAgentsNeeded()} to carry that one out.";
            status = "<color=#AE0000>UNDERSTAFFED</color>";
            int photoIndex = 7 + state.GetAgentsNeeded();
            photo1.Set("A LIL' TEAM\n\rLIKE THIS", WrongResultImg[photoIndex]);
            isNotCompleted = true;
            UploadBTN.SetActive(false);
            DisposeBTN.SetActive(true);
        }
        

        Hourtxt.text = $"OCT 30 - {timeComplete.Hour:00}:{timeComplete.Minute:00}";
        if(Name != "") ActionCalltxt.text = $"{actionVerb} \"{DeleteSpetialCharacter(Name).ToUpper()}\"";
        else { ActionCalltxt.text = $"{actionVerb}";}
        CheckTextOverflow();
        Statustxt.text = status;// + " at OCT 30th " + $"{timeComplete.Hour:00}:{timeComplete.Minute:00}";

        GetComponent<IndividualReportController>().SetType(false, word, report);

        if (isNotCompleted) return;
        if (report.GetDeleteDBRepoert() || report.GetIsTheLastReport()) btnText.transform.parent.gameObject.SetActive(false);
        Resulttxt.text = report.GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(Resulttxt, FindableBtnType.FindableBTN, report.FindableWords, false, false, true);
        GetComponent<IndividualReportController>().SetType(true, word, report);
        if(!report.GetIsAutomatic()) SetMaterial(materialUploadDB);

        SetPhotos(report);

    }

    void SetPhotos(ReportType report)
    {
        List<PhotoInfo> photoInfo = report.GetReportImage();

        if (report.GetDeleteDBRepoert())
        {
            photoQR.Set(photoInfo[0]?.text, photoInfo[0]?.photo);
            UploadBTN.gameObject.SetActive(false);
            DisposeBTN.gameObject.SetActive(false);
            return;
        }

        if (photoInfo.Count >= 1) photo1.Set(photoInfo[0]?.text, photoInfo[0]?.photo);
        if (photoInfo.Count >= 2) photo2.Set(photoInfo[1]?.text, photoInfo[1]?.photo);
        if (photoInfo.Count >= 3) photo3.Set(photoInfo[2]?.text, photoInfo[2]?.photo);
        if (photoInfo.Count >= 4) photo4.Set(photoInfo[3]?.text, photoInfo[3]?.photo);
        if (photoInfo.Count >= 5) photo5.Set(photoInfo[4]?.text, photoInfo[4]?.photo);

    }

    string DeleteSpetialCharacter(string txt)
    {
        return Regex.Replace(txt, @"[\?\.,\n\r]", "");
    }

    public void OnTakeReport(Component sender, object obj)
    {
        GetComponent<BoxCollider>().enabled = true;
        OnMovePaperToTakenPos?.Invoke(this, gameObject);
        if (!isNotCompleted) WordsManager.WM.RequestChangeStateSeen(word, report.GetState());
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
        photo4.gameObject.SetActive(false);
        photo5.gameObject.SetActive(false);

    }

    public Vector3 GetOutPos()
    {
        return OutPos.transform.position;
    }

    public void SetMaterial(Material material)
    {
        if (pageModel == null || material == null) return;

        var renderer = pageModel.GetComponent<Renderer>();
        if (renderer == null) return;

        renderer.material = material;
    }

    public StateEnum GetAction()
    { return report.GetAction(); }
    public bool GetIsAlreadyImposible()
    {
        return IsAlreadyImposible;
    }
}
