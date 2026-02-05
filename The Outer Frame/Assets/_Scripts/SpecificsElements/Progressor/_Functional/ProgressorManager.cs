using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] List<ProgressorModuleController> Slots = new List<ProgressorModuleController>();
    [SerializeField] int numOfSlots;
    [SerializeField] GameEvent OnProgressorSetSlot;
    [SerializeField] GameEvent ElementButtonClick;
    [SerializeField] GameEvent OnAnyAgenEnableGameOver;
    [SerializeField] GameEvent OnDisableInput;

    List<ProgressorModuleController> multipleAgentAction1 = new List<ProgressorModuleController>();
    List<ProgressorModuleController> multipleAgentAction2 = new List<ProgressorModuleController>();

    public void SetActionInCourse(Component c, object _data)
    {
        DataFromActionPlan data = (DataFromActionPlan)_data;
        StateEnum state = data.state;
        WordData _word = data.word;
        int agentsAmount = state.GetAgentsNeeded();
        if (state == null) return;
        if (state.GetSpecialActionWord()) _word = state.GetSpecialActionWord();

        if (GetUnusedSlot(1).Count == 0)
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
        int auxMultiActionNum = 1;
        bool multipleAgentAction1Used = false;
        bool multipleAgentAction2Used = false;
        int agentsAvaible = GetUnusedSlot(4).Count;

        List<ProgressorModuleController> SlotList = GetUnusedSlot(agentsAmount);

        SlotList.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));


        foreach (ProgressorModuleController slot in SlotList)
        {
            bool EnoughAgents = true;

      
            if (agentsAvaible < agentsAmount) EnoughAgents = false;


            string a = slot.name;
            Debug.Log(slot.name);

            slot.SetAction(_word, state, timeAction, auxMultiActionNum, EnoughAgents);

            auxMultiActionNum += 1;

            if (agentsAmount > 1)
            {
                if (!multipleAgentAction1Used)
                {
                    multipleAgentAction1.Add(slot);
                }
                else
                {
                    multipleAgentAction2.Add(slot);
                }
            }
        }

        if (multipleAgentAction1.Count != 0) multipleAgentAction1Used = true;
        if (multipleAgentAction2.Count != 0) multipleAgentAction2Used = true;

        OnProgressorSetSlot?.Invoke(this, false);

        if (GetUnusedSlot(1).Count == 0)
        {
            OnProgressorSetSlot?.Invoke(this, true);
        }
    }


    List<ProgressorModuleController> GetUnusedSlot(int amount)
    {
        List<ProgressorModuleController> AuxSlot = new List<ProgressorModuleController>();
        int auxAmount = 0;

        foreach (ProgressorModuleController slot in Slots)
        {
            if(!slot.GetIsFull())
            {
                AuxSlot.Add(slot);
                auxAmount += 1;
            }

            if (auxAmount == amount) break;
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

    //onAbortReport
    public void AbortMultiActionSlots(Component sender,object obj)
    {
        ProgressorModuleController slot = (ProgressorModuleController)obj;

        if(multipleAgentAction1.Contains(slot))
        {
            foreach(ProgressorModuleController s in multipleAgentAction1)
            {
                if(s != slot)
                {
                    s.PrintMultiAction();
                    s.GetSlot().CompleteMultiAction();
                }
            }
            multipleAgentAction1.Clear();
        }
        else if (multipleAgentAction2.Contains(slot))
        {

            multipleAgentAction2.Clear();
        }

    }


}
