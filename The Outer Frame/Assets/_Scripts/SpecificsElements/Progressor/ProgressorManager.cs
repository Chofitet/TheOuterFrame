using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] GameObject SlotPrefab;
    [SerializeField] PrinterController Printer;
    List<GameObject> Slots = new List<GameObject>();


    public void SetActionInCourse(string _word, string state)
    {
        Slots.RemoveAll(s => s == null);

        if (Slots.Count == 5)
        {
            Debug.LogWarning("Todos los Slots estan ocupados");
            return;
        }
        if (WordsManager.WM.CheckIfStateWasDone(_word, WordsManager.WM.ConvertStringToState(state)))
        {
            GameObject slot = Instantiate(SlotPrefab);
            slot.GetComponent<SlotController>().ActionWasDone();
            slot.transform.SetParent(transform, false);
            
        }
        else
        {
            GameObject slot = Instantiate(SlotPrefab);
            slot.GetComponent<SlotController>().initParameters(_word, state, GetMinutesOfAction(state),this);
            slot.transform.SetParent(transform, false);
            Slots.Add(slot);
            
        }

        for (int i = 0; i < Slots.Count; i++)
        {
            Debug.Log(Slots[i].name);
        }

    }

    public void ActionFinish(string word, GameObject slotReference)
    {
        Printer.InstanciateReport(word, slotReference);
    }


    int GetMinutesOfAction(string state)
    {
        switch (state)
        {
            case "dead":
             return 30;

            case "brainwashed":
                return 60;
        }
        return 0;
    }

}
