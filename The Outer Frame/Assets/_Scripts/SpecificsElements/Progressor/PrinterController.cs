using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
    [SerializeField] GameObject ReportPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] GameEvent OnTakeReport;
    [SerializeField] GameEvent OnResetProgressorSlot;
    [SerializeField] GameEvent OnFullPrinter;
    [SerializeField] float TimeLed;
    SlotController slot;

    public void PrintReport(Component component, object sc)
    {
        
        if (slot)
        {
            return;
        }
        OnFullPrinter?.Invoke(this, true);
        slot = (SlotController) sc;
        InstanciateReport(slot.gameObject);
    }

    public void InstanciateReport(GameObject slotReference)
    {
        GameObject report = Instantiate(ReportPrefab, InstanciateSpot.position,InstanciateSpot.rotation, InstanciateSpot);
        SlotController slotController = slotReference.GetComponent<SlotController>();
        report.GetComponent<ReportController>().initReport(slotController.GetWord(), slotController.GetReport(), slotController.GetIsAborted(), slotController.getisAlreadyDone(), slotController.GetIsTheSameAction(), slotController.GetIsOtherGroupActionDoing(), slotController.GetTimeComplete());
       
    }
    

    public void TakeReport(Component component, object obj)
    {
        OnResetProgressorSlot?.Invoke(this, slot.gameObject);
        OnFullPrinter?.Invoke(this, false);
        slot = null;
    }


}
