using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    Dictionary<string, Agent> Agents = new Dictionary<string , Agent>();
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

    public void RegisterAgent(string AgentName,Agent agent)
    {
        Agents.Add(GetAgentTypeWithState(AgentName), agent);
    }

    public void SetActiveOrDesactive(string state,bool x)
    {
        if (state == null) return;
        Agents[state].SetActiveOrDesactive(x);
    }

    public bool GetIfIsActive(string state)
    {
        return Agents[state].GetIfIsActive();
    }

    public List<string> GetInactiveAgents()
    {
        List<string> AuxAgentList = new List<string>();

        foreach (string key in Agents.Keys)
        {
            if (!Agents[key].GetIfIsActive())
            {
                AuxAgentList.Add(key);
            }
        }
        return AuxAgentList;

    }

    string GetAgentTypeWithState(string AgentName)
    {
        switch (AgentName)
        {
            case "hitman": return "dead";
            case "scientist": return "brainwashed";
        }

        return "none";
    }
}


