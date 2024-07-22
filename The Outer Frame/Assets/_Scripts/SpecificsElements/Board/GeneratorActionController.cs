using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorActionController : MonoBehaviour
{
    [SerializeField] List<StateEnum> ActionsToAdd = new List<StateEnum>();
    [SerializeField] List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters;
    [HideInInspector] [SerializeField] Transform BtnContainer;
    [HideInInspector] [SerializeField] GameObject BtnGeneratorIdeaPrefab;
    [HideInInspector] [SerializeField] GameObject Content;
    

    private void Start()
    {
        Content.SetActive(false);
    }


    public void OnAppearActionIdea(Component sender, object obj)
    {
        if (!CheckForConditionals()) return;
        Content.SetActive(true);

        List<StateEnum> ElementsToRemove = new List<StateEnum>();

        foreach (StateEnum action in ActionsToAdd)
        {
            GameObject auxInstance = Instantiate(BtnGeneratorIdeaPrefab, BtnContainer);
            auxInstance.GetComponent<BtnGenerateIdeaController>().Inicialization(action);
            ElementsToRemove.Add(action);
        }

        foreach (StateEnum action in ElementsToRemove) ActionsToAdd.Remove(action);

        
    }

    public bool CheckForConditionals()
    {
        if (Conditionals.Count == 0) return true;
        foreach (ScriptableObject conditional in Conditionals)
        {
            if (conditional is not IConditionable)
            {
                Debug.LogWarning(conditional.name + " is not a valid conditional");
                return false;
            }

            IConditionable auxConditional = conditional as IConditionable;

            if (!auxConditional.GetStateCondition())
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

        foreach (ScriptableObject conditional in Conditionals)
        {
            IConditionable auxConditional = conditional as IConditionable;

            if (auxConditional.CheckIfHaveTime())
            {
                nums.Add(auxConditional.GetTimeWhenWasComplete().GetTimeInNum());
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
