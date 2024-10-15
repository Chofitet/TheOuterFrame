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
    bool isDeleting;
    public void UpdatePC(Component sender, object obj)
    {
        WordData word = (WordData)obj;

        Datatxt.text = word.GetForm_DatabaseNameVersion();

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
