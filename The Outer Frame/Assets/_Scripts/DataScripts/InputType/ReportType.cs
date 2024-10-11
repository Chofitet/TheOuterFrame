using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Report", menuName = "Report")]
public class ReportType : ScriptableObject, IStateComparable
{
    [SerializeField] StateEnum Action;
    [SerializeField] StateEnum state;
    [SerializeField] bool isAutomatic;
    [SerializeField] int ChangeTimeOfAction;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string Text;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string TextForRepetition;
    [SerializeField] List<PhotoInfo> Photos = new List<PhotoInfo>();
    [SerializeField] List<ConditionalClass> Conditionals;
    [SerializeField] bool isOrderMatters;
    [SerializeField] bool XPosFix;
    [NonSerialized] bool wasSet;
    [NonSerialized] bool doing;
    [NonSerialized] bool wasRegisteredInDB;

    public StateEnum GetState() { return state;}
    public StateEnum GetAction() { return Action; }
    public string GetText() {  return Text;}
    public string GetTextForRepetition() { return TextForRepetition; }
    public int GetChangeTimeOfAction() { return ChangeTimeOfAction; }

    public List<PhotoInfo> GetReportImage() { return Photos; }

    private TimeData CompleteTime;
    public void SetTimeWhenWasDone()
    {
        CompleteTime = TimeManager.timeManager.GetTime();
    }

    private void OnEnable()
    {
        if (!state)
        {
            state = ScriptableObject.CreateInstance<StateEnum>();

            state.name = this.name + "_State";
        }
    }

    public TimeData GetTimeWhenWasDone() { return CompleteTime; }

    public bool GetWasSet() { return wasSet; }

    public bool SetWasSet() => wasSet = true;
    public bool GetIsAutomatic() { return isAutomatic;}

    public void SetDoing(bool x) { doing = x; }

    public bool getDoing() { return doing; }

    public void setwasRegisteredInDB() => wasRegisteredInDB = true;

    public bool GetwasRegisteredInDB() { return wasRegisteredInDB; }

    public bool GetTryCorrectXPos() { return XPosFix; }

    public bool CheckIfIsDefault()
    {
        if (!state) return false;
        if (Action == state)
        {
            return true;
        }
        else return false;
    }
    public bool CheckConditionals()
    {
        //fue mostrado ya 
        if (wasSet) return false;

        //es el estado default
        if (Conditionals == null) return true;

        foreach (ConditionalClass conditional in Conditionals)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition();

            Debug.Log("last compete conditional of "+ auxInterface +  " is " + auxInterface.GetLastCompletedConditional());

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else
        {
            return true;
        }
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in Conditionals)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            if (auxInterface.CheckIfHaveTime())
            {
                nums.Add(auxInterface.GetTimeWhenWasComplete().GetTimeInNum());
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
[Serializable]
public class ConditionalClass
{
    public ScriptableObject condition;
    public bool ifNot;

    [NonSerialized] public bool Doit;
    public void SetDoit(bool x = false)
    {
         Doit = x;
    }
}

    

[Serializable]
public class PhotoInfo
{
    public Sprite photo;
    public string text;
}
