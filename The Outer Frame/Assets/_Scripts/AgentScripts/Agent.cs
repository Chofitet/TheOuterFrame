using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private void Start()
    {
        AgentManager agentManager = GetComponentInParent<AgentManager>();
        agentManager.RegisterAgent(name, this);
    }

    bool isActive = true;

    public void SetActiveOrDesactive(bool x)
    {
        isActive = x;
    }

    public bool GetIfIsActive()
    {
        return isActive;
    }
}
