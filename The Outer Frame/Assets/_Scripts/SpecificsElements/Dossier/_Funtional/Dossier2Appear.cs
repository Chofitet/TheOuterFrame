using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dossier2Appear : MonoBehaviour
{
    [SerializeField] GameObject[] ToAppear;
    [SerializeField] List<ConditionalClass> Conditions = new List<ConditionalClass>();

    private void Start()
    {
       /* foreach (GameObject GO in ToAppear)
        {
            GO.SetActive(false);
        }*/
    }
    public void CheckIfDossier2IsAppear(Component sender, object obj)
    {
        if (CheckForConditionals())
        {
            foreach (GameObject GO in ToAppear)
            {
                GO.SetActive(true);
            }
        }
    }

    public bool CheckForConditionals()
    {

        if (Conditions == null) return true;

        foreach (ConditionalClass conditional in Conditions)
        {
            IConditionable auxInterface = conditional.condition as IConditionable;

            bool conditionState = auxInterface.GetStateCondition();

            if (!conditional.ifNot)
            {
                conditionState = !conditionState;
            }

            if (conditionState)
            {
                return false;
            }
        }

        return true;
    }
}
