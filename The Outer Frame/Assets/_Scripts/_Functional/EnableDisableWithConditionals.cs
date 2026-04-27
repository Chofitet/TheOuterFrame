using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableWithConditionals : MonoBehaviour
{
    [SerializeField] List<ConditionalClass> EnabledConditionals;
    [SerializeField] List<ConditionalClass> DisableConditionals;
    [SerializeField] GameObject ObjectToEnable;
    [SerializeField] int NumOfAlternativeConditional = 1;
    bool inactiveEnable;
    bool inactiveDisable;

    public void CheckForEnableConditionals(Component sender,object obj)
    {
        if (inactiveEnable) return;
        if(CheckForConditionals(EnabledConditionals))
        {
            ObjectToEnable.SetActive(true);
        }
    }

    public void CheckForDisableConditionals(Component sender, object obj)
    {
        if (inactiveDisable) return;
        if (CheckForConditionals(DisableConditionals))
        {
            ObjectToEnable.SetActive(false);
        }
    }

    public void InactiveEnable(Component sender,object obj)
    {
        inactiveEnable = true;
    }

    public void InactiveDisable(Component sender, object obj)
    {
        inactiveDisable = true;
    }

    public bool CheckForConditionals(List<ConditionalClass> ListOfConditionals)
    {
            if (ListOfConditionals.Count == 0) return true;

            foreach (ConditionalClass conditional in ListOfConditionals)
            {
                try
                {
                    IConditionable auxInterface = conditional.condition as IConditionable;

                    if (auxInterface == null)
                        throw new Exception("La condición no implementa IConditionable.");

                    bool conditionState = auxInterface.GetStateCondition(NumOfAlternativeConditional);

                    if (conditional.ifNot)
                    {
                        conditionState = !conditionState;
                    }

                    if (!conditionState)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error en el condicional: {conditional.condition.name}. Detalles: {ex.Message}", ex);
                }
            }

            return true;

       
    }

}
