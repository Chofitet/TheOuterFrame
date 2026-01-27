using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptsInDetailController : MonoBehaviour
{
    [SerializeField] GameObject panelCall;
    [SerializeField] TranscriptionCallController CallToFild;
    [SerializeField] GameEvent OnSearchWord;
    [SerializeField] GameEvent OnWikiWindow;
    [SerializeField] Scrollbar scrollbar;
    LogEntryData call;
    public void SetPanelText(Component sender, object obj)
    {
        panelCall.SetActive(true);

        call = (LogEntryData)obj;

        CallToFild.Inicialization(call.callType, call.word);
        scrollbar.value = 1;
    }

    public void QuitPanelReport()
    {
        panelCall.SetActive(false);
    }

    public void OnQuitPanel(Component sender, object obj)
    {
        panelCall.SetActive(false);
    }

    public void OnGoToSubject()
    {
        OnSearchWord?.Invoke(this, WordsManager.WM.FindWordWithPhoneNum(call.word));
        OnWikiWindow?.Invoke(this, null);
        QuitPanelReport();
    }
}
