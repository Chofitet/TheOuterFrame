using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCWikiController : MonoBehaviour
{
    [SerializeField] TMP_Text WikiData;
    [SerializeField] Image image;
    [SerializeField] TMP_Text PhoneNumber;
    [SerializeField] GameObject LockBTN;
    [SerializeField] TMP_Text LockField;

    DataBaseType input;

    public void CompleteFields(Component sender, object _wordData)
    {
        WordData wordData = (WordData)_wordData;

        input = WordsManager.WM.RequestBDWikiData(wordData);

        WikiData.text = "";
        image.sprite = null;
        PhoneNumber.text = "";

        CheckForUnlockCondition(input);

        StartCoroutine(Delay(input));
       
    }

    IEnumerator Delay(DataBaseType input)
    {
        yield return new WaitForSeconds(0.2f);
        if (input.GetText() != null) WikiData.text = input.GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(WikiData);
        HyperlinksManager.HLM.InstanciateHyperLink(WikiData);
        WikiData.GetComponent<TypingAnimText>().AnimateTyping();
        image.sprite = input.GetImage();
        PhoneNumber.text = input.GetPhoneNum();
        FindableWordsManager.FWM.InstanciateFindableWord(PhoneNumber);
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
}
