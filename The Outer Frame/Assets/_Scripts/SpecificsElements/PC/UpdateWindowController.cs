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
    bool isDeleting;
    public void UpdatePC(Component sender, object obj)
    {
        WordData word = (WordData)obj;

        Datatxt.text = word.GetForm_DatabaseNameVersion();

        if (isDeleting) SetDeletingPC();
    }

    public void DeletingPC(Component sender, object obj)
    {
        isDeleting = true;
    }

    void SetDeletingPC()
    {
        BackGround.color = Color.red;
        DeletingPanel.SetActive(true);
    }
}
