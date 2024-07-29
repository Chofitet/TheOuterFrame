using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] List<ProgressorModuleController> Slots = new List<ProgressorModuleController>();
    [SerializeField] int numOfSlots;
    [SerializeField] GameEvent OnProgressorSetSlot;
    

    public void SetActionInCourse(Component c, object _state)
    {
        WordData _word = WordSelectedInNotebook.Notebook.GetSelectedWord();
        StateEnum state = (StateEnum) _state;
        if (state.GetSpecialActionWord()) _word = state.GetSpecialActionWord();

        if (!GetUnusedSlot())
        {
            return;
        }

        GetUnusedSlot().SetAction(_word, state, WordsManager.WM.GetModifyActionDuration(_word,state));
        state.SetActiveOrDesactiveAgent(false);
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
}
