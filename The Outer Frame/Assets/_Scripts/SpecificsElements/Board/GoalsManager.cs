using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalsManager : MonoBehaviour
{
    [SerializeField] GameObject GoalRowPrefab;
    [SerializeField] GameEvent OnCheckGoalComplete;
    //[SerializeField] List<BoardGoals> GoalList = new List<BoardGoals>();
    [SerializeField] List<Goals> GoalList = new List<Goals>();

    public void CheckForGoalsToInstance(Component sender, object obj)
    {
        Invoke("InvokeOnCheckGoalComplete", 0.2f);
        if (GoalList.Count == 0) return;
        List<Goals> AuxListToRemove = new List<Goals>();
        foreach(Goals goal in GoalList)
        {
            if(goal.GetIsConditionalsToAdd())
            {
               GameObject aux = Instantiate(GoalRowPrefab, transform,false);
                aux.GetComponent<GoalRowController>().Inicialization(goal);
                AuxListToRemove.Add(goal);
            }
        }

        if (AuxListToRemove.Count == 0) return;
        foreach(Goals goals in AuxListToRemove)
        {
            GoalList.Remove(goals);
        }

    }

    void InvokeOnCheckGoalComplete() => OnCheckGoalComplete?.Invoke(this, null);
}

[Serializable]
public class Goals
{
    string name;
    [SerializeField] string Description;
    [SerializeField] List<ScriptableObject> ConditionalsToAdd = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters1;
    [SerializeField] List<ScriptableObject> ConditionalsToComplete = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters2;

    public void SetUp() { if (Description != "") name = Description; }

    public string GetDescription() { return Description; }

    public bool GetIsConditionalsToAdd() { return CheckForConditionals(ConditionalsToAdd, isOrderMatters1); }
    public bool GetIsConditionalsToComplete() { return CheckForConditionals(ConditionalsToComplete, isOrderMatters2); }

    public bool CheckForConditionals(List<ScriptableObject> list, bool isOrderMatters)
    {

        foreach (ScriptableObject conditional in list)
        {
            if (conditional is not IConditionable)
            {
                Debug.LogWarning(conditional.name + " is not a valid conditional");
                return false;
            }

            IConditionable auxConditional = conditional as IConditionable;

            if (!auxConditional.GetStateCondition())
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder(list);
        else return true;
    }

    bool CheckIfConditionalAreInOrder(List<ScriptableObject> list)
    {
        List<int> nums = new List<int>();

        foreach (ScriptableObject conditional in list)
        {
            IConditionable auxConditional = conditional as IConditionable;

            if (auxConditional.CheckIfHaveTime())
            {
                nums.Add(auxConditional.GetTimeWhenWasComplete().GetTimeInNum());
            }

        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return false;
            }
        }

        return true;
    }
}
