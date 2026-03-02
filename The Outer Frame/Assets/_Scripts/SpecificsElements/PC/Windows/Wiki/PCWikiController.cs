using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCWikiController : MonoBehaviour
{
    [SerializeField] TMP_Text WikiData;
    [SerializeField] Image image;
    [SerializeField] GameObject PhotoField;
    [SerializeField] GameObject LockBTN;
    [SerializeField] GameObject GooIdentificatorBTN; 
    [SerializeField] TMP_Text LockField;
    [SerializeField] RectTransform content;
    [SerializeField] List<GameObject> DataBaseFields = new List<GameObject>();
    [SerializeField] GameObject WikiInfoContent;
    [SerializeField] GameEvent OnPressGooIdentificatorBTN;
    List<GameObject> FIeldsInWikiInfo = new List<GameObject>();
    [SerializeField] GameObject LoadScreen;
    DataBaseType input;
    bool once;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (once) return;
        WikiInfoContent.SetActive(true);
        for(int i = 0; i< 15; i++)
        {
            GameObject fieldChild = WikiInfoContent.transform.GetChild(i).gameObject; 
            FIeldsInWikiInfo.Add(fieldChild);
            fieldChild.SetActive(false);
        }
        WikiInfoContent.SetActive(false);
        once = true;
    }

    public void CompleteFields(Component sender, object _wordData)
    {
        if (_wordData == null)
        {
            WikiData.text = "YOU SEARCH NOTHING YOU GET NOTHING";
            FindableWordsManager.FWM.InstanciateFindableWord(WikiData, FindableBtnType.FindableBTN);

            HyperlinksManager.HLM.InstanciateHyperLink(WikiData, FindableBtnType.HyperLink);

            InstanciateRedactedBlock.IRM.InstanciateRedactedBlocks(WikiData);
            return;
        }
        WordData wordData = (WordData)_wordData;

        input = WordsManager.WM.RequestBDWikiData(wordData);
        if(isInPCView) input.SetWasSearched();

        foreach (Transform child in WikiData.transform)
        {
            Destroy(child.gameObject);
        }


        WikiData.text = "";
        

        image.sprite = null;
        //PhoneNumber.text = "";

        CheckForUnlockCondition(input);
        WikiInfoContent.SetActive(false);

        StartCoroutine(Delay(input));


    }

    IEnumerator Delay(DataBaseType input)
    {
        LoadScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        WikiInfoContent.SetActive(true);
        if (input.GetText() != null) WikiData.text = input.GetText();
        
        FindableWordsManager.FWM.InstanciateFindableWord(WikiData, FindableBtnType.FindableBTN,input.FindableWords);
        HyperlinksManager.HLM.InstanciateHyperLink(WikiData, FindableBtnType.HyperLink, input.HyperLinks);
        InstanciateRedactedBlock.IRM.InstanciateRedactedBlocks(WikiData,input.RedactedBlocks, true);
        //WikiInfoContent.SetActive(true);
        image.sprite = input.GetImage();
        CompleteFields();
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, WikiData.GetComponent<RectTransform>().sizeDelta.y);
        if (input.GetImage() == null)
        {
            PhotoField.SetActive(false);
        }
        else PhotoField.gameObject.SetActive(true);

        WikiInfoContent.SetActive(CheckFieldsInWikiInfoContent());

        LoadScreen.SetActive(false);

        //PhoneNumber.text = input.GetPhoneNum();
        //FindableWordsManager.FWM.InstanciateFindableWord(PhoneNumber);
    }

    private void DelayLoadCrossBar()
    {
        WikiData.ForceMeshUpdate();
    }

    bool CheckForUnlockCondition(DataBaseType DB)
    {
        if(DB.GetIsLocked())
        {
            GooIdentificatorBTN.SetActive(false);
            LockField.gameObject.SetActive(true);
            LockBTN.SetActive(true);
            return true;
        }
        else if(DB.GetIsGooIdentificator())
        {
            GooIdentificatorBTN.SetActive(true);
            LockField.gameObject.SetActive(false);
            LockBTN.SetActive(false);
            return true;
        }
        else
        {
            GooIdentificatorBTN.SetActive(false);
            LockField.gameObject.SetActive(false);
            LockBTN.SetActive(false);
            //LockField.text = "Unlock Cargo Doors";
        }

        return false;
    }

    public void PressUnlock()
    {
        if(CheckAreLocked())
        {
            LockField.text = "CARGO DOOR: UNLOCKED";
            LockBTN.GetComponent<Button>().interactable = false;
            WordsManager.WM.AddStateOnHistory(input.GetwordToUnlock(), input.GetUnlockState());
            WordsManager.WM.AddStateOnSeenHistory(input.GetwordToUnlock(), input.GetUnlockState());
        }
    }

    public void PressGooIdentificatorBTN()
    {
        OnPressGooIdentificatorBTN?.Invoke(this, null);
    }

    bool CheckAreLocked()
    {
        List<StateEnum> auxHistory = input.GetwordToUnlock().GetHistory();

        foreach (StateEnum state in auxHistory)
        {
            if (state == input.GetUnlockState())
            {
                return false;
            }
        }

        return true;
    }

    void CompleteFields()
    {
        Dictionary<int,string> auxDictionary = input.GetDataFieldsInfo();
        if (auxDictionary.Count == 0) return;
        for(int i = 0; i < DataBaseFields.Count; i++)
        {
            if(auxDictionary[i] == "")
            {
                DataBaseFields[i].SetActive(false);
                continue;
            }
            DataBaseFields[i].SetActive(true);
            TMP_Text auxText = DataBaseFields[i].transform.GetChild(1).GetComponent<TMP_Text>();
            auxText.text = auxDictionary[i];
            InstanciateRedactedBlock.IRM.InstanciateRedactedBlocks(auxText,null,false,true);
            FindableWordsManager.FWM.InstanciateFindableWord(DataBaseFields[i].transform.GetChild(1).GetComponent<TMP_Text>(),FindableBtnType.FindableBTN);
            HyperlinksManager.HLM.InstanciateHyperLink(DataBaseFields[i].transform.GetChild(1).GetComponent<TMP_Text>(), FindableBtnType.HyperLink);
        }
    }

    bool isInPCView;

    public void CheckPCView(Component sender, object obj)
    {
        ViewStates view = (ViewStates)obj;

        if (view == ViewStates.PCView)
        {
            isInPCView = true;
            if(input) input.SetWasSearched();
        }
        else isInPCView = false;

    }

    bool CheckFieldsInWikiInfoContent()
    {

        foreach(GameObject field in FIeldsInWikiInfo)
        {
            if (field.activeSelf)
            {
                return true;
            }
        }

        return false;

    }

    bool CheckTranscriptOrReportUploaded()
    {
        return true;
    }


}
