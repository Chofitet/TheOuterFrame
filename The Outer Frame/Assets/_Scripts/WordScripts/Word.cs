using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word : MonoBehaviour
{
    //Clase de la que heredan los scripts de las palabras, acá estaran las funcionalidades comunes entre todas ellas
    [SerializeField] WordData SO;
    private List<WordState> stateHistory = new List<WordState>();
    private Dictionary<WordState, TimeData> stateTimeHistory = new Dictionary<WordState, TimeData>();
    private WordState currentState = WordState.none;

    private void Start()
    {
        WordsManager WM = GetComponentInParent<WordsManager>();
        WM.RegisterWord(name, this);
    }

    public enum WordState
    {
        none,
        dead,
        brainwashed,
        hacked,
        investigated

    };

    public void ChangeState(WordState newState)
    {
        if (CheckIfStateWasDone(newState))
        {
            Debug.LogWarning("The state " + newState.ToString() + " was done");
            return;
        }
        currentState = newState;
        stateHistory.Add(currentState);
        stateTimeHistory.Add(currentState, TimeManager.timeManager.GetTime());
    }

    public string GetActionPlanResult()
    {
        return SO.GetActionPlanResult(currentState);
    }

    public string GetLastTVNew()
    {
        return SO.GetTVNews(currentState);
    }

    public string GetDataBD()
    {
        return SO.GetDataBD();
    }

    public string RequestInputAccordingState(WordState state)
    {
        return SO.GetActionPlanResult(state);
    }

    public Dictionary<WordState,TimeData> GetStateTimeHistory()
    {
        return stateTimeHistory;
    }

    public bool CheckIfStateWasDone(WordState state)
    {
        for(int i = 0; i < stateHistory.Count; i++)
        {
            if (stateHistory[i] == state)
            {
                return true;
            }
            
        }
        return false;
    }

}
