using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
    [SerializeField] GameObject ReportPrefab;
    [SerializeField] Transform InstanciateSpot;
    List<SlotController> SlotsInQueue = new List<SlotController>();
    [SerializeField] GameEvent OnTakeReport;

    public void AddToQueue(Component component, object sc)
    {
        SlotController slot = (SlotController)sc;

        SlotsInQueue.Add(slot);
        if (SlotsInQueue.Count == 1)
        {
            InstanciateReport(slot.gameObject);
        }

        foreach(SlotController scs in SlotsInQueue)
        {
            Debug.Log(scs.GetWord().GetName());
        }
    }

    public void InstanciateReport(GameObject slotReference)
    {
        GetComponent<BoxCollider>().enabled = true;
        GameObject report = Instantiate(ReportPrefab, InstanciateSpot.position,InstanciateSpot.rotation, InstanciateSpot);
        report.GetComponent<ReportController>().initReport(slotReference.GetComponent<SlotController>().GetWord());
    }
    private void OnMouseUpAsButton()
    {
        GetComponent<BoxCollider>().enabled = false;
        if (GetIsQueueFree()) return;
        Destroy(SlotsInQueue[0].gameObject);
        SlotsInQueue.Remove(SlotsInQueue[0]);
        OnTakeReport?.Invoke(this, null);
        if (GetIsQueueFree()) return;
        InstanciateReport(SlotsInQueue[0].gameObject);
    }

    bool GetIsQueueFree()
    {
        if (SlotsInQueue.Count == 0) return true;
        else return false;
    }
}
