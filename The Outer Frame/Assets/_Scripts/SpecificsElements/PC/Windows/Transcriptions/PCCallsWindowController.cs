using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PCCallsWindowController : MonoBehaviour
{
    [SerializeField] GameObject PrefabBtCall;
    [SerializeField] GameObject Grid;
    [SerializeField] GameObject panelCall;
    [SerializeField] TranscriptionCallController CallToFild;
    [SerializeField] GameEvent OnEnableTranscriptionPCBtn;
    bool isDeleted;
    WordData word;

    //OnSearchWord
    public void GetWord(Component sender, object _word)
    {
        if (_word == null) return;
        word = (WordData)_word;
        checkHaveCallsToShow();
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
        
        if (isDeleted) return;
        List<CallType> CallsHistory = WordsManager.WM.GetAllCathedCalls(word);

        foreach (var call in CallsHistory)
        {
            GameObject btn = Instantiate(PrefabBtCall, Grid.transform, false);
            btn.GetComponent<PCCallController>().Inicialization(call);
        }
    }

    void checkHaveCallsToShow()
    {
        List<CallType> CallsHistory = WordsManager.WM.GetAllCathedCalls(word);

        foreach (var call in CallsHistory)
        {
            OnEnableTranscriptionPCBtn?.Invoke(this, null);
        }
    }

    public void SetPanelText(Component sender, object obj)
    {
        panelCall.SetActive(true);

        CallType call = (CallType)obj;

        CallToFild.Inicialization(call,word);
    }

    public void QuitPanelReport()
    {
        panelCall.SetActive(false);
    }

    public void OnQuitPanel(Component sender, object obj)
    {
        panelCall.SetActive(false);
    }

    public void DeleteAllReports(Component sender, object obj)
    {
        isDeleted = true;
    }
}
