using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverWhitAlertLevelDirector : MonoBehaviour
{
    [SerializeField] GameEvent OnButtonElementClick;
    [SerializeField] GameEvent OnChangeScene;
    [SerializeField] GameEvent OnFadeOutSoundGameOverWhitEmergencyNew;
    [SerializeField] GameEvent OnFadeOutSoundGameOverWhitNormalNew;
    [SerializeField] float TimeToChangeToTVAutomatlyFromGeneralView;
    [SerializeField] float TimeToChangeToTVAutomatlyFromAnotherView;
    [SerializeField] float TimeToLeaveLevelWithAnEmergencyNew;
    [SerializeField] float TimeToLeaveLevelWithANormalNew;
    bool once;
    bool isInGameOver;
    float TimeToChangeToTVAutomatly;
    [SerializeField] bool noTriggerLose;
    public void OnGameOver(Component sender, object obj)
   {
        if (noTriggerLose) return;
        DelayGoToTVCoroutine = StartCoroutine(DelayGoToTV());
        isInGameOver = true;
   }

    Coroutine DelayGoToTVCoroutine;
    IEnumerator DelayGoToTV()
    {
        yield return new WaitForSeconds(TimeToChangeToTVAutomatly);
        StartDelayToFadeOutLevel();
    }

    ViewStates lastView;

    public void CheckView(Component sender, object obj)
    {
        ViewStates currentView = (ViewStates)obj;

        Debug.Log($"last view {lastView}");

        if (lastView == ViewStates.GeneralView)
        {
            TimeToChangeToTVAutomatly = TimeToChangeToTVAutomatlyFromGeneralView;
        }
        else
        {
            TimeToChangeToTVAutomatly = TimeToChangeToTVAutomatlyFromAnotherView;
        }

        if (currentView == ViewStates.TVView)
        {
            if (DelayGoToTVCoroutine != null) StopCoroutine(DelayGoToTVCoroutine);
            StartDelayToFadeOutLevel();
        }

        lastView = currentView;
    }

    public void StartDelayToFadeOutLevel()
    {
        if (!once && isInGameOver)
        {
            OnButtonElementClick?.Invoke(this, ViewStates.TVView);
            StartCoroutine(DelayToFadeOutLevel());
            once = true;
        }
    }

    IEnumerator DelayToFadeOutLevel()
    {
        
        if(SecondsToFadeOutLevel == TimeToLeaveLevelWithAnEmergencyNew)
        {
            Debug.Log("Leave with Emergency New");
            OnFadeOutSoundGameOverWhitEmergencyNew?.Invoke(this, null);
        }
        else if(SecondsToFadeOutLevel == TimeToLeaveLevelWithANormalNew)
        {
            Debug.Log("Leave with Normal New");
            OnFadeOutSoundGameOverWhitNormalNew?.Invoke(this, null);
        }

        yield return new WaitForSeconds(SecondsToFadeOutLevel);
        OnChangeScene?.Invoke(this, "LoseMenu");
    }

    float SecondsToFadeOutLevel;

    public void OnGetNewOnTV(Component sender,object obj)
    {
        INewType lastNew = (INewType)obj;

        if (lastNew.GetIfIsAEmergency())
        {
            SecondsToFadeOutLevel = TimeToLeaveLevelWithAnEmergencyNew;
        }
        else SecondsToFadeOutLevel = TimeToLeaveLevelWithANormalNew;
    }
}
