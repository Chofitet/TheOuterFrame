public interface IConditionable 
{
    bool GetStateCondition();

    bool CheckIfHaveTime();
    TimeData GetTimeWhenWasComplete();
}
