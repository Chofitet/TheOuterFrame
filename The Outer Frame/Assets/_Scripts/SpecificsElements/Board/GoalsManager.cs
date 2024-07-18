using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalsManager : MonoBehaviour
{
    [SerializeField] GameObject GoalRowPrefab;
    [SerializeField] GameEvent OnCheckGoalComplete;
    [SerializeField] List<BoardGoals> GoalList = new List<BoardGoals>();

    public void CheckForGoalsToInstance(Component sender, object obj)
    {
        Invoke("InvokeOnCheckGoalComplete", 0.2f);
        if (GoalList.Count == 0) return;
        List<BoardGoals> AuxListToRemove = new List<BoardGoals>();
        foreach(BoardGoals goal in GoalList)
        {
            if(goal.GetIsConditionalsToAdd())
            {
               GameObject aux = Instantiate(GoalRowPrefab, transform,false);
                aux.GetComponent<GoalRowController>().Inicialization(goal);
                AuxListToRemove.Add(goal);
            }
        }

        if (AuxListToRemove.Count == 0) return;
        foreach(BoardGoals goals in AuxListToRemove)
        {
            GoalList.Remove(goals);
        }

    }

    void InvokeOnCheckGoalComplete() => OnCheckGoalComplete?.Invoke(this, null);
}
