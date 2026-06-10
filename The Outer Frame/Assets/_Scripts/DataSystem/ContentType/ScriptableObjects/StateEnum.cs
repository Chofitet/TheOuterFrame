using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New State", menuName = "State")]
public class StateEnum : DataType, IReseteableScriptableObject
{
    [Header("Action Fields")]
    [SerializeField] string InfinitiveVerb;
    [SerializeField] string Actioning;
    [SerializeField] string Actioned;
    [SerializeField] int AgentsNeeded = 1;
    [SerializeField] int TimeToComplete;
    [SerializeField] int TimeToShowNew;
    [SerializeField] string observationtxt = "";
    [SerializeField] bool NeedWordLocation;
    [Header("Idea In Progressor")]
    [SerializeField] WordData IdeaWordData;
    [SerializeField] string IdeaVerb;
    [SerializeField] string IdeaInfinitiveVerb;
    [SerializeField] string IdeaWord;


    
    
    [NonSerialized] bool isDone;
    [NonSerialized] bool isWrittenOnAP;
    [HideInInspector][SerializeField] List<ConditionalClass> InactiveConditionals = new List<ConditionalClass>();


    public override void ResetScriptableObject()
    {
        isDone = false;
        isWrittenOnAP = false;
    }

    public int GetTime() { return TimeToComplete; }
    public int GetTimeToShowNew() { return TimeToShowNew; }
    public string GetInfinitiveVerb() { return InfinitiveVerb; }
    public string GetActioningVerb() { return Actioning; }
    public string GetActionedVerb() { return Actioned; }

    public string GetIdeaInfinitiveVerb() { return IdeaInfinitiveVerb; }

    public WordData GetSpecialActionWord() { return IdeaWordData; }

    public string GetSpeticialActionWordName()
    {
        if (IdeaWord == "") return IdeaWordData.GetProgressorNameVersion();
        else return IdeaWord;
    }

    public string GetObservationTxt() { return observationtxt; }

    public string GetIdeaVerb() { return IdeaVerb; }

    public void SetIsDone(bool x)
    {
        isDone = x;
        MarkDirty();
    }

    public bool GetIsDone() { return isDone; }

    public bool GetIfIsActive()
    {
        return true;
    }

    public int GetAgentsNeeded()
    {
        return AgentsNeeded;
    }

    public void SetisWrittenOnAP(bool x)
    {
        isWrittenOnAP = x;
        MarkDirty();
    }

    public bool GetisWrittenOnAP() { return isWrittenOnAP; }

    public bool GetNeedWordLocation() { return NeedWordLocation; }

    public bool GetInactiveConditionals()
    {
        // chequeo si la acción ya no es realizable (solo funcional para las ideas)

        bool x = false;

        if (!GetSpecialActionWord()) return x;

        if(InactiveConditionals.Count == 0) return x;

        x = CheckForConditionals(InactiveConditionals);

        return x;

    }

    public bool CheckForConditionals(List<ConditionalClass> ListOfConditionals)
    {
        try
        {
            if (ListOfConditionals.Count == 0) return true;

            foreach (ConditionalClass conditional in ListOfConditionals)
            {
                try
                {
                    IConditionable auxInterface = conditional.condition as IConditionable;

                    if (auxInterface == null)
                        throw new Exception("La condición no implementa IConditionable.");

                    bool conditionState = auxInterface.GetStateCondition(1);

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

            if (false) return CheckIfConditionalAreInOrder(ListOfConditionals);
            else return true;
        }
        catch (Exception ex)
        {
            // Mensaje de error general con la excepción específica
            Debug.LogError($"Error al evaluar los condicionales. Detalles: {ex.Message}");
            return false;
        }
    }

    bool CheckIfConditionalAreInOrder(List<ConditionalClass> ListOfConditionals)
    {
        List<int> nums = new List<int>();

        foreach (ConditionalClass conditional in ListOfConditionals)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            if (auxInterface.CheckIfHaveTime())
            {
                nums.Add(auxInterface.GetTimeWhenWasComplete().GetTimeInNum());
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


    public void SetInactiveConditional(List<ConditionalClass> list)
    {
        InactiveConditionals = list;
    }

}


