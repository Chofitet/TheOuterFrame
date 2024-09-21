public interface IConditionable 
{
    bool GetStateCondition();
    bool GetAlternativeConditional();
    bool CheckIfHaveTime();
    TimeData GetTimeWhenWasComplete();
    IConditionable GetLastCompletedConditional();
    int GetTimeToShowNews();
}
