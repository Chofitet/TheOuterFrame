using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "New Agent", menuName = "Agent")]
public class Agent : ScriptableObject
{
    [SerializeField] string Name;
    [SerializeField] Sprite Icon;

    [NonSerialized] bool isActive = true;

    public bool GetIsActive() { return isActive; }

    public void SetActiveDesactive(bool x) => isActive = x;
}
