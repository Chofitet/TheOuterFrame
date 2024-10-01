using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCWikiController : MonoBehaviour
{
    [SerializeField] TMP_Text WikiData;
    [SerializeField] Image image;
    [SerializeField] GameObject LockBTN;
    [SerializeField] TMP_Text LockField;
    [SerializeField] List<GameObject> DataBaseFields = new List<GameObject>();

    DataBaseType input;

    public void CompleteFields(Component sender, object _wordData)
    {
        WordData wordData = (WordData)_wordData;

        input = WordsManager.WM.RequestBDWikiData(wordData);

        foreach (Transform child in WikiData.transform)
        {
            Destroy(child.gameObject);
        }


        WikiData.text = "";
        
        image.sprite = null;
        //PhoneNumber.text = "";

        CheckForUnlockCondition(input);

        StartCoroutine(Delay(input));


    }

    IEnumerator Delay(DataBaseType input)
    {

        yield return new WaitForSeconds(0.2f);
        if (input.GetText() != null) WikiData.text = input.GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(WikiData);
        HyperlinksManager.HLM.InstanciateHyperLink(WikiData);
        InstanciateRedactedBlock.IRM.InstanciateRedactedBlocks(WikiData);
        image.sprite = input.GetImage();
        CompleteFields();
        //PhoneNumber.text = input.GetPhoneNum();
        //FindableWordsManager.FWM.InstanciateFindableWord(PhoneNumber);
    }

    bool CheckForUnlockCondition(DataBaseType DB)
    {
        if(DB.GetIsLocked())
        {
            LockField.gameObject.SetActive(true);
            LockBTN.SetActive(true);
            return true;
        }
        else
        {
            LockField.gameObject.SetActive(false);
            LockBTN.SetActive(false);
            LockField.text = "Unlock Cargo Doors";
        }

        return false;
    }

    public void PressUnlock()
    {
        if(CheckAreLocked())
        {
            LockField.text = "Lock Cargo Doors";
            WordsManager.WM.AddStateOnHistory(input.GetwordToUnlock(), input.GetUnlockState());
        }
        else
        {
            LockField.text = "Unlock Cargo Doors";
            WordsManager.WM.CleanStateOnHistory(input.GetwordToUnlock(), input.GetUnlockState());
        }
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
                return;
            }
            DataBaseFields[i].SetActive(true);
            DataBaseFields[i].transform.GetChild(1).GetComponent<TMP_Text>().text = auxDictionary[i];
            FindableWordsManager.FWM.InstanciateFindableWord(DataBaseFields[i].transform.GetChild(1).GetComponent<TMP_Text>());
        }
    }
}
