using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] List<ProgressorModuleController> Slots = new List<ProgressorModuleController>();

    public void SetActionInCourse(Component c, object _state)
    {
        WordData _word = WordSelectedInNotebook.Notebook.GetSelectedWord();
        StateEnum state = (StateEnum) _state;

        if (!GetUnusedSlot()) return;

        GetUnusedSlot().SetAction(_word, state, state.GetTime());
        AgentManager.AM.SetActiveOrDesactive(state, false);
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
