using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pre_ProccessFindableWords : MonoBehaviour
{
    [SerializeField] ContentType contentTypeTest;
    [SerializeField] FindableWordsManager findableWordsManager;
    [SerializeField] WordData Irrelevant;

    [Header("Report Type")]
    [SerializeField] TMP_Text reportField;

    [Header("Transcription Type")]
    [SerializeField] TMP_Text transcriptionField;

    [Header("DB Type")]
    [SerializeField] TMP_Text DBField;

    [Header("TV Type")]
    [SerializeField] TMP_Text TVTitleField;
    [SerializeField] TMP_Text TV2LinesTitleField;
    [SerializeField] TMP_Text TVTextField;

    public void ProcessInEditor()
    {
        Debug.Log("Processing Findable Words in Editor...");

        if (contentTypeTest == null)
        {
            Debug.LogWarning("No ContentType assigned.");
            return;
        }

        // Acá después podés llamar a tu sistema real
        Debug.Log($"Processing content of type: {contentTypeTest.GetType().Name}");

        if(contentTypeTest.GetType().Name == "ReportType")
        {
            proccessContent(reportField);
        }
    }

    public void proccessContent(TMP_Text textField)
    {
        textField.text = contentTypeTest.GetText();

        textField.ForceMeshUpdate();

        contentTypeTest.SetFindableWords(ProccessFindableWord.SearchForFindableWord(textField, Irrelevant));


    }

}
