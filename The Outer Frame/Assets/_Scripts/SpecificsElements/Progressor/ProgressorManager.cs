using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressorManager : MonoBehaviour
{

    [SerializeField] GameObject SlotPrefab;
    [SerializeField] PrinterController Printer;
    [SerializeField] List<Transform> Anchors = new List<Transform>();
    Dictionary<Transform,GameObject> UsedAnchors = new Dictionary<Transform, GameObject>();
    List<GameObject> Slots = new List<GameObject>();

    bool IsPossibleSetASlot()
    {
        Slots.RemoveAll(s => s == null);
        RemoveDeletedSlots();
        bool auxbool = false;

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
            Transform FreeAnchor = GetUnusedSlot();
            GameObject slot = Instantiate(SlotPrefab, FreeAnchor.position, FreeAnchor.rotation,transform);
            UsedAnchors.Add(FreeAnchor, slot);
            slot.GetComponent<SlotController>().ActionWasDone();
            slot.transform.SetParent(transform, false);
            
        }
        else
        {
            Transform FreeAnchor = GetUnusedSlot();
            GameObject slot = Instantiate(SlotPrefab, FreeAnchor.position, FreeAnchor.rotation,transform);
            UsedAnchors.Add(FreeAnchor, slot);
            slot.GetComponent<SlotController>().initParameters(_word, state, state.GetTime());
            slot.transform.SetParent(transform, false);
            Slots.Add(slot);
            AgentManager.AM.SetActiveOrDesactive(state, false);
            
        }

    }

    void RemoveDeletedSlots()
    {
        List<Transform> keysToRemove = new List<Transform>();

        foreach (Transform key in UsedAnchors.Keys)
        {
            if (UsedAnchors[key] == null)
            {
                keysToRemove.Add(key);
            }
        }

        foreach (Transform key in keysToRemove)
        {
            UsedAnchors.Remove(key);
        }
    }

    Transform GetUnusedSlot()
    {
        Transform aux = null;

        foreach(Transform slot in Anchors)
        {
            GameObject gameSlot = null;
            UsedAnchors.TryGetValue(slot, out gameSlot);

            if (!gameSlot) return slot;
        }

        return aux;
    }

}
