using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfilesController : MonoBehaviour
{
    [SerializeField] GameObject buttonNewGame;
    [SerializeField] GameObject buttonRetry;

    private void Start()
    {
        if(DataPersistenceManager.instance.HasGameData())
        {
            buttonNewGame.SetActive(true);
            buttonRetry.GetComponent<RectTransform>().localPosition = new Vector3(0, 26.6f, 0);
            buttonRetry.transform.GetChild(0).GetComponent<TMP_Text>().text = "RETRY";
            buttonRetry.transform.GetComponent<TriggerArrayOfEvents>().ChangeSomeStringToPass("LoadingScreen");
            buttonNewGame.GetComponent<Button>().onClick.AddListener(() => DataPersistenceManager.instance.DeleteProfileData("0"));
            buttonNewGame.GetComponent<Button>().onClick.AddListener(DataPersistenceManager.instance.NewGame);
            buttonNewGame.GetComponent<Button>().onClick.AddListener(DataPersistenceManager.instance.SaveGame);
        }
        else
        {
            
           // buttonRetry.GetComponent<Button>().onClick.AddListener(DataPersistenceManager.instance.NewGame);
           // buttonRetry.GetComponent<Button>().onClick.AddListener(DataPersistenceManager.instance.SaveGame);

        }

    }
}
