using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
    [SerializeField] GameObject ReportPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] GameEvent OnTakeReport;
    [SerializeField] GameEvent OnFullPrinter;
    [SerializeField] BlinkEffect Led;
    [SerializeField] float TimeLed;
    SlotController slot;

    public void PrintReport(Component component, object sc)
    {
        
        if (slot)
        {
            IsFull();
            return;
        }
        OnFullPrinter?.Invoke(this, true);
        slot = (SlotController) sc;
        InstanciateReport(slot.gameObject);
    }

    public void InstanciateReport(GameObject slotReference)
    {
        GetComponent<BoxCollider>().enabled = true;
        GameObject report = Instantiate(ReportPrefab, InstanciateSpot.position,InstanciateSpot.rotation, InstanciateSpot);
        SlotController slotController = slotReference.GetComponent<SlotController>();
        report.GetComponent<ReportController>().initReport(slotController.GetWord(), slotController.GetReport(), slotController.GetIsAborted(), slotController.getisAlreadyDone(), slotController.GetTimeComplete());
        IsPrinting();
    }
    private void OnMouseUpAsButton()
    {
        GetComponent<BoxCollider>().enabled = false;
        OnTakeReport?.Invoke(this, slot.gameObject);
        OnFullPrinter?.Invoke(this, false);
        slot = null;
    }

    void IsFull()
    {
        Led.TurnOnLigth(null, Color.red);
        Invoke("TurnOffLed", TimeLed);
    }

    void IsPrinting()
    {
        Led.TurnOnLigth(null, Color.green);
        Invoke("TurnOffLed", TimeLed);
    }

    void TurnOffLed()
    {
        Led.TurnOffLigth(null, null);
    }

}
