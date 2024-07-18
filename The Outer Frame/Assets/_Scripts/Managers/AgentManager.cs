using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] List<Agent> Agents = new List<Agent>();
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

    public void SetActiveOrDesactive(Agent agent,bool x)
    {
        x = x == true ? false : true;

        if (agent == null) return;
        FindAgentInList(agent).SetActiveDesactive(x);
    }

    public bool GetIfIsActive(Agent state)
    {
        return FindAgentInList(state).GetIsActive();
    }

    public List<Agent> GetAgentList() { return Agents; }

    public List<Agent> GetInactiveAgents()
    {
        List<Agent> AuxAgentList = new List<Agent>();

        foreach (Agent agent in Agents)
        {
            if (!agent.GetIsActive())
            {
                AuxAgentList.Add(agent);
            }
        }
        return AuxAgentList;

    }

    Agent FindAgentInList(Agent agent)
    {
        Agent aux = agent; 
        foreach(Agent _agent in Agents)
        {
            if(_agent == agent)
            {
                aux = _agent;
            }
        }

        return aux;
    }

}


