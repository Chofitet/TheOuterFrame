using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class GeneratorActionController : MonoBehaviour, IPlacedOnBoard
{
    [SerializeField] List<StateEnum> ActionsToAdd = new List<StateEnum>();
    [SerializeField] List<StringConnectionController> StringConnections = new List<StringConnectionController>();
    [SerializeField] List<ConditionalClass> Conditionals = new List<ConditionalClass>();
    [SerializeField] bool isOrderMatters;
    [SerializeField] List<ConditionalClass> InactiveConditionals = new List<ConditionalClass>();
    [SerializeField] bool isAFailedIdea;
    [HideInInspector] [SerializeField] BtnGenerateIdeaController BtnGeneratorIdeaPrefab;
    [HideInInspector][SerializeField] GameObject Content;
    [HideInInspector] [SerializeField] GameEvent OnMoveObjectToPapersPos;
    [HideInInspector] [SerializeField] GameObject CheckImage;
    [HideInInspector] [SerializeField] GameEvent OnMoveToCornerIdea;
    [HideInInspector] [SerializeField] GameEvent OnMoveToOutOfView;
    [HideInInspector] [SerializeField] GameEvent OnSetTakenPosit;
    [HideInInspector] [SerializeField] GameObject CroosIcon;
    bool ActionIsDoing;
    bool isDone;
    private bool istaken;
    GameObject IdeaButtom;

    private void Start()
    {
        Content.SetActive(false);
    }

    public bool GetConditionalState()
    {

        if (StringConnections.Count != 0)
        {
            foreach (StringConnectionController connection in StringConnections)
            {
                if (!connection.GetIsConnected()) return false;
            }
        }

        if (CheckForConditionals(Conditionals))
        {
            OnAppearActionIdea();
            return true;
        }

        return false;
    }

    private void OnMouseUpAsButton()
    {
        GetComponent<BoxCollider>().enabled = false;
        istaken = true;
        OnSetTakenPosit?.Invoke(null, gameObject);
    }

    public void Reset(Component sender, object obj)
    {
        istaken = false;
        GetComponent<BoxCollider>().enabled = true;
    }

    public void OnAppearActionIdea()
    {
        if (!CheckForConditionals(Conditionals)) return;
       
        Content.SetActive(true);

        BtnGeneratorIdeaPrefab.Inicialization(ActionsToAdd[0]);
        IdeaButtom = BtnGeneratorIdeaPrefab.gameObject;
    }

    public bool IsOutOfBoard()
    {
        CheckIfDone();
        if (isDone) return false;
        if (InactiveConditionals.Count == 0) return false;
        if (!CheckForConditionals(InactiveConditionals)) return false;

        GetComponent<BoxCollider>().enabled = false;
        return true;
    }


    void CheckIfDone()
    {
        if (!IdeaButtom) return;

        BtnGenerateIdeaController btn = IdeaButtom.GetComponent<BtnGenerateIdeaController>();
        
        
        if (btn.GetState().GetSpecialActionWord().CheckIfStateSeenWasDone(btn.GetState()))
        {
            if (!isAFailedIdea) CheckImage.SetActive(true);
            else CroosIcon.SetActive(true);
            btn.InactiveIdea();
            isDone = true;
        }
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
                        throw new Exception("La condici�n no implementa IConditionable.");

                    bool conditionState = auxInterface.GetStateCondition(2);

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

            if (isOrderMatters) return CheckIfConditionalAreInOrder(ListOfConditionals);
            else return true;
        }
        catch (Exception ex)
        {
            // Mensaje de error general con la excepci�n espec�fica
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

    public void MoveToCorner()
    {
        OnMoveToCornerIdea?.Invoke(null, gameObject);
        GetComponent<MoveBoardElementsToPos>().SetIsOutOfBoard(true);
    }

    public void CheckAportActionIdea(Component sender, object obj)
    {
        StateEnum action = (StateEnum)obj;
        if (ActionsToAdd[0] == action)
        {
            GetComponent<MoveBoardElementsToPos>().SetIsOutOfBoard(false);
            ActionIsDoing = false;
        }
    }

    public void IsAddingOtherAction(Component sender, object obj)
    {
        if (ActionIsDoing) return;
        if (istaken) return;
        StateEnum action = (StateEnum)obj;
        if (ActionsToAdd[0] != action)
        {
            GetComponent<MoveBoardElementsToPos>().SetIsOutOfBoard(false);
        }
    }

    public void IsActionRejected(Component sender, object obj)
    {
        StateEnum action = (StateEnum)obj;
        if (ActionsToAdd[0] == action)
        {
            GetComponent<MoveBoardElementsToPos>().SetIsOutOfBoard(false);
            ActionIsDoing = false;
        }
    }

    public void SetActionIsDoing(Component sender, object obj)
    {
        DataFromActionPlan data = (DataFromActionPlan)obj;
        StateEnum action = data.state;
        if (ActionsToAdd[0] == action) ActionIsDoing = true;
    }

    public bool ActiveInBegining()
    {
        return false;
    }

    public bool GetIsTaken()
    {
        return istaken;
    }
}
