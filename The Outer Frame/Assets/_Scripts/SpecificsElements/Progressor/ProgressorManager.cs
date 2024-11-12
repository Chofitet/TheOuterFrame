using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] List<ProgressorModuleController> Slots = new List<ProgressorModuleController>();
    [SerializeField] int numOfSlots;
    [SerializeField] GameEvent OnProgressorSetSlot;
    [SerializeField] GameEvent ElementButtonClick;
    [SerializeField] GameEvent OnAnyAgenEnableGameOver;
    [SerializeField] GameEvent OnDisableInput;

    public void SetActionInCourse(Component c, object _data)
    {
        DataFromActionPlan data = (DataFromActionPlan)_data;
        StateEnum state = data.state;
        WordData _word = data.word;
        if (state == null) return;
        if (state.GetSpecialActionWord()) _word = state.GetSpecialActionWord();

        if (!GetUnusedSlot())
        {
            OnProgressorSetSlot?.Invoke(this, true);
            return;
        }

        if(!WordsManager.WM.RequestReport(_word, state))
        {
            Debug.LogWarning("No " + state.GetInfinitiveVerb() + " reports to show in " + _word.GetName());
            return;
        }
        int timeAction = Mathf.Abs(state.GetTime() + WordsManager.WM.RequestReport(_word, state).GetChangeTimeOfAction());
        GetUnusedSlot().SetAction(_word, state, timeAction);
        OnProgressorSetSlot?.Invoke(this, false);

        if (!GetUnusedSlot())
        {
            OnProgressorSetSlot?.Invoke(this, true);
        }
    }


    ProgressorModuleController GetUnusedSlot()
    {
        ProgressorModuleController AuxSlot = null;
        foreach (ProgressorModuleController slot in Slots)
        {
            if(!slot.GetIsFull())
            {
                return slot;
            }
        }

        return AuxSlot;
    }

    public void DisableSlot(Component sender, object obj)
    {
        GameObject slot = (GameObject)obj;
        ProgressorModuleController SlotToRemove = slot.GetComponent<ProgressorModuleController>();
        Slots.Remove(SlotToRemove);
    }

    public void CheckAllAgentsDown(Component sender, object obj)
    {
        if (Slots.Count == 0)
        {
            ElementButtonClick?.Invoke(this, ViewStates.GameOverView);
            OnAnyAgenEnableGameOver?.Invoke(this, null);
            OnDisableInput?.Invoke(this, null);
        }
    }

    [ContextMenu("Test Game Over")]
    public void TestGameOver()
    {
        ElementButtonClick?.Invoke(this, ViewStates.GameOverView);
        OnAnyAgenEnableGameOver?.Invoke(this, null);
        OnDisableInput?.Invoke(this, null);
    }

    public void DisableAllExept(Component sender, object obj)
    {
        StateEnum finalState = (StateEnum)obj;

        List<ProgressorModuleController> RemoveList = new List<ProgressorModuleController>();

        foreach (ProgressorModuleController s in Slots)
        {
            if (finalState == s.GetState()) continue;
            RemoveList.Add(s);
        }

        Slots.RemoveAll(slot => RemoveList.Contains(slot));
        OnProgressorSetSlot?.Invoke(this, true);
    }
}
