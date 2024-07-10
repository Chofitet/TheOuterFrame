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
    }

    public void InstanciateReport(GameObject slotReference)
    {
        GetComponent<BoxCollider>().enabled = true;
        GameObject report = Instantiate(ReportPrefab, InstanciateSpot.position,InstanciateSpot.rotation, InstanciateSpot);
        SlotController slotController = slotReference.GetComponent<SlotController>();
        report.GetComponent<ReportController>().initReport(slotController.GetWord(), slotController.GetState(), slotController.GetIsAborted(), slotController.GetIsNotPossible());
    }
    private void OnMouseUpAsButton()
    {
        GetComponent<BoxCollider>().enabled = false;
        if (GetIsQueueFree()) return;
        OnTakeReport?.Invoke(this, SlotsInQueue[0].gameObject);
        SlotsInQueue.Remove(SlotsInQueue[0]);
        if (GetIsQueueFree()) return;
        InstanciateReport(SlotsInQueue[0].gameObject);
    }

    bool GetIsQueueFree()
    {
        if (SlotsInQueue.Count == 0) return true;
        else return false;
    }
}
