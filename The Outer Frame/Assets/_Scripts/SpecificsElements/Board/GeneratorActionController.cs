using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GeneratorActionController : MonoBehaviour, IPlacedOnBoard
{
    [SerializeField] List<StateEnum> ActionsToAdd = new List<StateEnum>();
    [SerializeField] List<StringConnectionController> StringConnections = new List<StringConnectionController>();
    [SerializeField] List<ConditionalClass> Conditionals = new List<ConditionalClass>();
    [SerializeField] bool isOrderMatters;
    [SerializeField] List<ConditionalClass> InactiveConditionals = new List<ConditionalClass>();
    [SerializeField] Transform BtnContainer;
    [SerializeField] GameObject BtnGeneratorIdeaPrefab;
    [SerializeField] GameObject Content;
    [SerializeField] GameEvent OnMoveObjectToPapersPos;
    [SerializeField] GameObject CheckImage;
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

        List<StateEnum> ElementsToRemove = new List<StateEnum>();

        foreach (StateEnum action in ActionsToAdd)
        {
            GameObject auxInstance = Instantiate(BtnGeneratorIdeaPrefab, BtnContainer);
            auxInstance.GetComponent<BtnGenerateIdeaController>().Inicialization(action);
            ElementsToRemove.Add(action);
            IdeaButtom = auxInstance;
        }

        foreach (StateEnum action in ElementsToRemove) ActionsToAdd.Remove(action);
    }

    public bool IsOutOfBoard()
    {
        CheckIfDone();
        if (isDone) return false;
        if (InactiveConditionals.Count == 0) return false;
        if (!CheckForConditionals(InactiveConditionals)) return false;

        return true;
    }

    void CheckIfDone()
    {
        if (!IdeaButtom) return;

        BtnGenerateIdeaController btn = IdeaButtom.GetComponent<BtnGenerateIdeaController>();

        if (btn.GetState().GetIsDone())
       {
            CheckImage.SetActive(true);
            btn.InactiveIdea();
            isDone = true;
       }
    }
    public bool CheckForConditionals(List<ConditionalClass> ListOfConditionals)
    {
        if (ListOfConditionals.Count == 0) return true;
        foreach (ConditionalClass conditional in ListOfConditionals)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetAlternativeConditional();

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

        if (isOrderMatters) return CheckIfConditionalAreInOrder(ListOfConditionals);
        else return true;
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

    

    public bool ActiveInBegining()
    {
        return false;
    }

    public bool GetIsTaken()
    {
        return istaken;
    }
}
