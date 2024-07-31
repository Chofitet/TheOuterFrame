using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCCallsWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabBtCall;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject panelCall;

    WordData word;

    //OnSearchWord
    public void GetWord(Component sender, object _word)
    {
        word = (WordData)_word;
    }

    //OnCalltWindow
    public void InstanciateBtnCall(Component sender, object _word)
    {
        if (!word) return;
        foreach (Transform child in Grid.GetComponentsInChildren<Transform>())
        {
            if (child != Grid.transform)
            {
                Destroy(child.gameObject);
            }
        }

        List<CallType> CallsHistory = WordsManager.WM.GetAllCathedCalls(word);

        foreach (var call in CallsHistory)
        {
            GameObject btn = Instantiate(PrefabBtCall, Grid.transform, false);
            btn.GetComponent<PCCallController>().Inicialization(call);
        }
    }

    public void SetPanelText(Component sender, object obj)
    {
        panelCall.SetActive(true);

        string call = (string)obj;

        TMP_Text panelText = panelCall.transform.GetChild(0).GetComponent<TMP_Text>();
        panelText.text = call;
        FindableWordsManager.FWM.InstanciateFindableWord(panelText);
    }

    public void QuitPanelReport()
    {
        panelCall.SetActive(false);
    }
}
