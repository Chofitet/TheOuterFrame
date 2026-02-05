using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PrinterController : MonoBehaviour
{
    [SerializeField] GameObject ReportPrefab;
    [SerializeField] GameObject CandyPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] Transform OutSpot;
    [SerializeField] Transform CandyOutSpot;
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
        SlotController slotController = slotReference.GetComponent<SlotController>();
        GameObject Object = null;
        if (slotController.GetObjectType() == ObjectToPrint.report || slotController.GetNoComplete())
        {
            Object = Instantiate(ReportPrefab, InstanciateSpot.position, InstanciateSpot.rotation, InstanciateSpot);
            Object.GetComponent<ReportController>().initReport(slotController.GetWord(), slotController.GetReport(), slotController.GetIsAborted(), slotController.getisAlreadyDone(), slotController.GetIsTheSameAction(), slotController.GetIsOtherGroupActionDoing(), slotController.GetIsAlreadyImposible(), slotController.GetTimeComplete(), slotController.GetIsAVilifyBlockedAction(), slotController.GetAreNotEnoughAgents());
            Object.transform.DOMove(OutSpot.position, 0.2f).SetEase(Ease.OutSine);
        }
        else if(slotController.GetObjectType() == ObjectToPrint.Candy1 || slotController.GetObjectType() == ObjectToPrint.Candy2)
        {
            Object = Instantiate(CandyPrefab, InstanciateSpot.position, InstanciateSpot.rotation, InstanciateSpot);
            Object.GetComponent<CandyController>().initCandy(slotController.GetWord(), slotController.GetReport(), slotController.GetIsAborted(), slotController.getisAlreadyDone(), slotController.GetIsTheSameAction());
            Object.GetComponent<CandyStateController>().InitializeCandy(slotController.GetObjectType());
            Object.transform.DOMove(CandyOutSpot.position, 0.2f).SetEase(Ease.OutSine);
            Object.transform.DORotate(CandyOutSpot.rotation.eulerAngles, 0.2f).SetEase(Ease.InQuart);
        }


       
    }
    

    public void TakeReport(Component component, object obj)
    {
        if(slot) OnResetProgressorSlot?.Invoke(this, slot.gameObject);
        OnFullPrinter?.Invoke(this, false);
        slot = null;
    }


}
