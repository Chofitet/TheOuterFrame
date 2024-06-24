using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
    [SerializeField] GameObject ReportPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] Transform FinalPosReport;
    [SerializeField] ProgressorManager PM;
    public void InstanciateReport(GameObject slotReference)
    {
        GameObject report = Instantiate(ReportPrefab, InstanciateSpot.position,InstanciateSpot.rotation,transform);
        report.GetComponent<ReportController>().initReport(slotReference.GetComponent<SlotController>().GetWord());
        GetComponent<SmoothMoveObjectToPoints>().SetObjectToMove(report.transform);
    }

    private void OnMouseUpAsButton()
    {

        PM.TakeLastReport();
    }
}
