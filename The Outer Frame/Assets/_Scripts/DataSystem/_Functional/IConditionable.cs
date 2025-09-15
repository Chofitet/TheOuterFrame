public interface IConditionable 
{
    bool GetStateCondition(int NumOfAlternativeConditional = 1);
    bool CheckIfHaveTime();
    TimeData GetTimeWhenWasComplete();
    IConditionable GetLastCompletedConditional();
    int GetTimeToShowNews();
}
