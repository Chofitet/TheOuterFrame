using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Call", menuName ="Input/Calls")]
public class CallType : ScriptableObject
{
    //Opcion m�s viable: Poder marcar la fecha de la llamada, entonces el recorrido se va a hacer comprobando si los condicionales se cumplen, de haber dos se colocar� el que tenga la fecha m�s pr�xima a la actual.

    [Header("Time Zone")]
    [SerializeField] TimeCheckConditional StartTime;
    [SerializeField] TimeCheckConditional EndTime;
    [SerializeField] string Dialogue;

    [SerializeField] List<IConditionable> Conditionals = new List<IConditionable>();
    [SerializeField] bool isOrderMatters;

    [NonSerialized] private bool isCatch;

    public string GetDialogue() { return Dialogue; }
    public void SetCached() => isCatch = true;
    public bool GetIsCatch() { return isCatch; }
    public bool CheckForTimeZone()
    {
        if (StartTime.GetStateCondition() && EndTime.GetStateCondition()) return true;
        else return false;
    }

    public bool CheckForConditionals()
    {
        foreach (IConditionable conditional in Conditionals)
        {
            if (!conditional.GetStateCondition())
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder();
        else return true;
    }

    bool CheckIfConditionalAreInOrder()
    {
        List<int> nums = new List<int>();

        foreach (IConditionable conditional in Conditionals)
        {
            if (conditional.CheckIfHaveTime())
            {
                nums.Add(conditional.GetTimeWhenWasComplete().GetTimeInNum());
            }

        }

        for (int i = 0; i < nums.Count - 1; i++)
        {
            if (nums[i] > nums[i + 1])
            {
                return false;
            }
        }

        return true;
    }

}
