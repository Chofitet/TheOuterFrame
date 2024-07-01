using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] List<StateEnum> Agents = new List<StateEnum>();
    public static AgentManager AM { get; private set; }
    private void Awake()
    {

        if (AM != null && AM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            AM = this;
        }

    }

    public void SetActiveOrDesactive(StateEnum state,bool x)
    {
        x = x == true ? false : true;

        if (state == null) return;
        FindAgentInList(state).SetActiveOrDesactive(x);
    }

    public bool GetIfIsActive(StateEnum state)
    {
        return FindAgentInList(state).GetIfIsActive();
    }

    public List<StateEnum> GetAgentList() { return Agents; }

    public List<StateEnum> GetInactiveAgents()
    {
        List<StateEnum> AuxAgentList = new List<StateEnum>();

        foreach (StateEnum agent in Agents)
        {
            if (!agent.GetIfIsActive())
            {
                AuxAgentList.Add(agent);
            }
        }
        return AuxAgentList;

    }

    StateEnum FindAgentInList(StateEnum state)
    {
        StateEnum aux = state; 
        foreach(StateEnum agent in Agents)
        {
            if(agent == state)
            {
                aux = agent;
            }
        }

        return aux;
    }

}


