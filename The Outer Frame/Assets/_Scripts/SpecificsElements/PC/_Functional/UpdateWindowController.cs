using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateWindowController : MonoBehaviour
{
    [SerializeField] TMP_Text Datatxt;
    [SerializeField] GameObject DeletingPanel;
    [SerializeField] Image BackGround;
    [SerializeField] GameEvent OnWikiWindow;
    [SerializeField] Image DatabaseImage;
    [SerializeField] Sprite NoImage;
    [SerializeField] TMP_Text PaperType;
    bool isDeleting;
    public void UpdatePC(Component sender, object obj)
    {
        WordData word = (WordData)obj;

        Datatxt.text = WordsManager.WM.FindWordWithPhoneNum(word).GetForm_DatabaseNameVersion();

        Sprite _image = WordsManager.WM.RequestBDWikiData(word).GetImage();
        if (_image == null) DatabaseImage.sprite = NoImage;
        else DatabaseImage.sprite = _image;

        bool isAReport = sender.gameObject.GetComponent<ReportController>() != null ? true : false;

        PaperType.text = isAReport ? "REPORT TO:" : "TRANSCRIPTION TO:";

        if (isDeleting)
        {
            SetDeletingPC();
            OnWikiWindow?.Invoke(this, null);
        }
    }

    public void DeletingPC(Component sender, object obj)
    {
        isDeleting = true;
    }

    void SetDeletingPC()
    {
        DeletingPanel.SetActive(true);
    }

}
