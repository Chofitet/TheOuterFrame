using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestOptionsController : MonoBehaviour
{
    [SerializeField] Toggle RetryOption;
    [SerializeField] Toggle LeaveOption;
    [SerializeField] Toggle SureOption;
    [SerializeField] Button StampBTN;
    [SerializeField] GameEvent OnLeaveGame;
    [SerializeField] GameEvent ChangeScene;
    bool isInTutorial;

    bool isLeaveOptionCheck;
    bool isSureOptionCheck;

    public void CheckLeaveToggle(bool toggle)
    {
        isLeaveOptionCheck = toggle;
        EnableStampBTN();
    }

    public void CheckSureToggle(bool toggle)
    {
        isSureOptionCheck = toggle;
        EnableStampBTN();
    }

    public void EnableStampBTN()
    {
        if(isLeaveOptionCheck && isSureOptionCheck)
        {
            StampBTN.enabled = true;
        }
        else StampBTN.enabled = false;
    }

    public void OnPressStampBTN()
    {
        if (RetryOption.isOn)
        {
            StartCoroutine(DelayChangeScene("LoadingScreen"));
        }
        else if (LeaveOption.isOn)
        {
            StartCoroutine(DelayChangeScene("MainMenu"));
        }

        RetryOption.enabled = false;
        LeaveOption.enabled = false;
        SureOption.enabled = false;

        OnLeaveGame?.Invoke(null, null);
    }

    IEnumerator DelayChangeScene(string scene)
    {
        yield return new WaitForSeconds(1f);
        ChangeScene?.Invoke(this, scene);
    }

    public void ResetMenu(Component sender, object obj)
    {
        RetryOption.isOn = false;
        LeaveOption.isOn = false;
        SureOption.isOn = false;
    }

    public void SetIsInTutorial(Component sender, object obj)
    {
        isInTutorial = (bool)obj;

       /* if (isInTutorial) RetryOption.gameObject.SetActive(false);
        else RetryOption.gameObject.SetActive(true);*/
    }
}
