using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
    [SerializeField] GameObject ReportPrefab;
    [SerializeField] Transform InstanciateSpot;
    [SerializeField] Transform FinalPosReport;
    GameObject LastSlot;
    public void InstanciateReport(string word, GameObject slotReference)
    {
        GameObject report = Instantiate(ReportPrefab, InstanciateSpot.position,InstanciateSpot.rotation,transform);
        report.GetComponent<ReportController>().initReport(word);
        GetComponent<SmoothMoveObjectToPoints>().SetObjectToMove(report.transform);
        LastSlot = slotReference;
    }

    private void OnMouseUpAsButton()
    {
        if (LastSlot == null) return;
        Destroy(LastSlot);
    }
}
