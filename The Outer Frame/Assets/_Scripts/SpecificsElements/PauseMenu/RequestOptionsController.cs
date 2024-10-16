using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestOptionsController : MonoBehaviour
{
    [SerializeField] Toggle RetryOption;
    [SerializeField] Toggle LeaveOption;
    [SerializeField] GameEvent OnLeaveGame;
    [SerializeField] GameEvent ChangeScene;

    public void OnPressStampBTN()
    {
        if (RetryOption.isOn)
        {
            StartCoroutine(DelayChangeScene("Level1"));
        }
        else if (LeaveOption.isOn)
        {
            StartCoroutine(DelayChangeScene("MainMenu"));
        }

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
    }
}
