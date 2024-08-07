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

    public void CompleteFields(Component sender, object _wordData)
    {
        WordData wordData = (WordData)_wordData;

        DataBaseType input = WordsManager.WM.RequestBDWikiData(wordData);

        WikiData.text = "";
        image.sprite = null;
        PhoneNumber.text = "";

        StartCoroutine(Delay(input));
       
    }

    IEnumerator Delay(DataBaseType input)
    {
        yield return new WaitForSeconds(0.2f);
        if (input.GetText() != null) WikiData.text = input.GetText();
        FindableWordsManager.FWM.InstanciateFindableWord(WikiData);
        WikiData.GetComponent<TypingAnimText>().AnimateTyping();
        image.sprite = input.GetImage();
        PhoneNumber.text = input.GetPhoneNum();
        FindableWordsManager.FWM.InstanciateFindableWord(PhoneNumber);
    }
}
