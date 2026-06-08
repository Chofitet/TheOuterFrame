using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfilesController : MonoBehaviour
{
    [SerializeField] GameObject buttonContinue;
    [SerializeField] GameObject Credits_LeavePanel;

    private void Start()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            DataPersistenceManager.instance.NewGame();
            DataPersistenceManager.instance.SaveGame();
        }
        else
        {
            DataPersistenceManager.instance.ChangeSelectedProfileId("0");
        }

        if(!DataPersistenceManager.instance.GetGameData().ContingencyContinue)
        {
            Credits_LeavePanel.transform.localPosition = new Vector3(Credits_LeavePanel.transform.localPosition.x, -13.3f, Credits_LeavePanel.transform.localPosition.x);
            buttonContinue.SetActive(false);
        }

    }

    public void ResetSaveData()
    {
       DataPersistenceManager.instance.ResetSpecificFields("0", false, true);
    }
}
