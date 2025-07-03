using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class StringConnectionController : MonoBehaviour
{
    [SerializeField] MoveBoardElementsToPos Node1;
    [SerializeField] MoveBoardElementsToPos Node2;
    

    [SerializeField] GameObject AnimPin1;
    Vector3 startPosPin1;
    [SerializeField] GameObject AnimPin2;
    Vector3 startPosPin2;
    [SerializeField] GameEvent OnPuttingStringSound;
    [SerializeField] List<ScriptableObject> Conditionals = new List<ScriptableObject>();
    [SerializeField] bool isOrderMatters;
    GameObject content;
    bool isConnected;

    private void Start()
    {
        content = transform.GetChild(0).gameObject;
        content.SetActive(false);

        if (!Node1 || !Node2)
        {
            Debug.LogWarning("Board connection " + name + " dont have a conection node assigned");
            return;
        }

        startPosPin1 = AnimPin1.transform.position;
        startPosPin2 = AnimPin2.transform.position;

    }

    public void UpdatePositionRotation(Component sender, object obj)
    {
        startPosPin1 = AnimPin1.transform.position;
        startPosPin2 = AnimPin2.transform.position;
    }

    public void CheckConnection(Component sender, object obj)
    {
       if (Node1.GetIsPlaced() && Node2.GetIsPlaced() && CheckForConditionals())
        {
            AnimPin2.transform.position = startPosPin1;
            content.SetActive(true);
            AnimPin2.transform.DOMove(startPosPin2, 0.5f).SetEase(Ease.InOutQuad);
            if(!isConnected) OnPuttingStringSound?.Invoke(this, null);
            isConnected = true;
        }
    }

    public bool GetIsConnected() { return isConnected; }

    public bool CheckForConditionals()
    {

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
