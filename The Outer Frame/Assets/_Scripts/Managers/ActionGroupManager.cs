using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionGroupManager : MonoBehaviour
{
    public static ActionGroupManager AGM { get; private set; }

    private void Awake()
    {
        if (AGM != null && AGM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            AGM = this;
        }
    }

    [SerializeField] List<ActionGroup> ListOfActionGroups = new List<ActionGroup>();

    [SerializeField] List<StateEnum> ListOfAcionsInterruptCalls = new List<StateEnum>();

    public bool ChekAreInTheSameGroup(WordData word, StateEnum newAction)
    {
        List<ActionGroup> AuxListOfActionGroups = new List<ActionGroup>();

        //filter types
        foreach(ActionGroup AG in ListOfActionGroups)
        {
            if(AG.type == word.GetActionGroupType())
            {
                AuxListOfActionGroups.Add(AG);
            }
        }

        foreach(ActionGroup AG in AuxListOfActionGroups)
        {
            List<StateEnum> actions = AG.Actions;
            foreach(StateEnum action in actions)
            {
                if (word.CheckIfActionIsDoing(action)) return true;
            }
        }

        return false;
    }

    public bool CheckForPhoneActionInterrupted(WordData word)
    {
        foreach(StateEnum state in ListOfAcionsInterruptCalls)
        {
            if (word.CheckIfActionIsDoing(state)) return true;
        }

        return false;
    }


}

[Serializable]
class ActionGroup
{
    public ActionGoupType type;
    public List<StateEnum> Actions = new List<StateEnum>();
}
public enum ActionGoupType
{
    Thing,
    Person,
}
