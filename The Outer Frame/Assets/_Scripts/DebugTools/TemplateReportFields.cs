using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateReportFields : MonoBehaviour
{
    [SerializeField] StateEnum Action;
    [SerializeField] StateEnum state;
    [SerializeField] bool isAutomatic;
    [SerializeField] float ChangeTimeOfAction;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string Text;
    [SerializeField] [TextArea(minLines: 3, maxLines: 10)] string TextForRepetition;
    [SerializeField] List<ConditionalClass> Conditionals;
}

