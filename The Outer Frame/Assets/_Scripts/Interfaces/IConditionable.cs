public interface IConditionable 
{
    bool GetStateCondition();
    bool GetAlternativeConditional();
    bool CheckIfHaveTime();
    TimeData GetTimeWhenWasComplete();
}
