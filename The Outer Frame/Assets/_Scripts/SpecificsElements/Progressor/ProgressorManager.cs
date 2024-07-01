using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] GameObject SlotPrefab;
    [SerializeField] PrinterController Printer;
    List<GameObject> Slots = new List<GameObject>();


    public bool IsPossibleSetASlot()
    {

        Slots.RemoveAll(s => s == null);
        bool auxbool = false;

        foreach (GameObject slot in Slots)
        {
            if (slot.GetComponent<SlotController>().IsActionComplete()) auxbool = true;
        }


        if (Slots.Count == 5)
        {
            Debug.LogWarning("Todos los Slots estan ocupados");
            return false;
        }
        else if (auxbool) return false;
        else return true;

    }


    public void SetActionInCourse(Component c, object _state)
    {
        WordData _word = WordSelectedInNotebook.Notebook.GetSelectedWord();
        StateEnum state = (StateEnum) _state;

        if (!IsPossibleSetASlot()) return;

        if (WordsManager.WM.CheckIfStateWasDone(_word, state))
        {
            GameObject slot = Instantiate(SlotPrefab);
            slot.GetComponent<SlotController>().ActionWasDone();
            slot.transform.SetParent(transform, false);
            
        }
        else
        {
            GameObject slot = Instantiate(SlotPrefab);
            slot.GetComponent<SlotController>().initParameters(_word, state, state.GetTime(),this);
            slot.transform.SetParent(transform, false);
            Slots.Add(slot);
            AgentManager.AM.SetActiveOrDesactive(state, false);
            
        }

        for (int i = 0; i < Slots.Count; i++)
        {
            Debug.Log(Slots[i].name);
        }

    }

    public void ActionFinish(GameObject slotReference)
    {
        Printer.InstanciateReport(slotReference);
    }

    public void TakeLastReport()
    {
        Slots.RemoveAll(s => s == null);
        if (Slots.Count == 0) return;
        Slots[0].GetComponent<SlotController>().CleanSlot();
        
    }

}
